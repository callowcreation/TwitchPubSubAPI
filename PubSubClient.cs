#region Author
/*
     
     Jones St. Lewis Cropper (caLLow)
     
     Another caLLowCreation
     
     Visit us on Google+ and other social media outlets @caLLowCreation
     
     Thanks for using our product.
     
     Send questions/comments/concerns/requests to 
      e-mail: caLLowCreation@gmail.com
      subject: TwitchPubSubAPI
     
*/
#endregion

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchPubSubAPI.Payloads;
using TwitchPubSubAPI.Payloads.Response;
using TwitchPubSubAPI.Payloads.Request;
using System.Text.RegularExpressions;

namespace TwitchPubSubAPI
{
    public class PubSubClient
    {
        const int HEARTBEAT_DELAY_TIME_MS = 1000 * 60 * 4;
        //const int HEARTBEAT_DELAY_TIME_MS = 1000 * 30;           
        const int MAX_BACKOFF_THRESHOLD_INTERVAL = 1000 * 60 * 2;
        const int BACKOFF_THRESHOLD_INTERVAL = 1000 * 3; //ms to wait before reconnect

        const int MAX_PONG_WAIT_INTERVAL = 1000 * 10;

        static int s_ReconnectInterval = BACKOFF_THRESHOLD_INTERVAL;

        static Random s_Rnd = new Random();
        
        static ClientWebSocket s_WebSocket;
        static BlockingCollection<string> s_MessageQueue;
        static PingRequest s_PingRequest;
        static bool s_PongReceived;

        static CancellationTokenSource s_CancellationSource;
        static CancellationTokenSource s_SendCancellationSource;
        static CancellationTokenSource s_ProcessCancellationSource;

        static Dictionary<string, Action<IPayload>> s_TopicEvents = new Dictionary<string, Action<IPayload>>()
        {
            {"channel-bits-events-v1", (obj) => s_OnChannelBits?.Invoke(Convert<Bits>(obj)) },
            {"channel-bits-events-v2", (obj) => s_OnChannelBits?.Invoke(Convert<Bits>(obj)) },
            /*unTested*/{"channel-bits-badge-unlocks", (obj) => s_OnChannelBitsBadge?.Invoke(Convert<BitsBadge>(obj)) },
            {"channel-points-channel-v1", (obj) => s_OnChannelPoints?.Invoke(Convert<Points>(obj)) },
            /*unTested*/{"channel-subscribe-events-v1", (obj) => s_OnChannelSubscriptions?.Invoke(Convert<Subscriptions>(obj)) },
            {"chat_moderator_actions", (obj) => s_OnChannelChatModeratorActions?.Invoke(Convert<ChatModeratorActions>(obj)) },
            {"whispers", (obj) => s_OnChannelWhispers?.Invoke(Convert<Whispers>(obj)) },
        };

        static string[] s_Scopes = null;

        static string s_ListenNonce = string.Empty;
        static string s_Message = string.Empty;

        static int pingSentCounter = 0;

        static event Action<Bits> s_OnChannelBits;
        static event Action<BitsBadge> s_OnChannelBitsBadge;
        static event Action<Points> s_OnChannelPoints;
        static event Action<Subscriptions> s_OnChannelSubscriptions;
        static event Action<ChatModeratorActions> s_OnChannelChatModeratorActions;
        static event Action<Whispers> s_OnChannelWhispers;
        
        public static event Action OnServerRestart;
        public static event Action<PayloadResponse> OnResponse;
        public static event Action<Payload> OnPayload;
        public static event Action<ReconnectReason> OnReconnectRequired;


        public class DebugException : EventArgs
        {
            public readonly Exception exception;
            public readonly string reason;

            public DebugException(Exception exception, string reason)
            {
                this.exception = exception;
                this.reason = reason;
            }
        }

        public static event Action<DebugException> OnTaskException;
        
        public static event Action<Bits> OnBits
        {
            add
            {
                ValidateOrThrow("bits:read", nameof(OnBits));
                s_OnChannelBits += value;
            }
            remove { s_OnChannelBits -= value; }
        }

        public static event Action<BitsBadge> OnBitsBadge
        {
            add
            {
                ValidateOrThrow("bits:read", nameof(OnBitsBadge));
                s_OnChannelBitsBadge += value;
            }
            remove { s_OnChannelBitsBadge -= value; }
        }

        public static event Action<Points> OnPoints
        {
            add
            {
                ValidateOrThrow("channel:read:redemptions", nameof(OnPoints));
                s_OnChannelPoints += value;
            }
            remove { s_OnChannelPoints -= value; }
        }

        public static event Action<Subscriptions> OnSubscriptions
        {
            add
            {
                ValidateOrThrow("channel_subscriptions", nameof(OnSubscriptions));
                s_OnChannelSubscriptions += value;
            }
            remove { s_OnChannelSubscriptions -= value; }
        }

        public static event Action<ChatModeratorActions> OnChatModeratorActions
        {
            add
            {
                ValidateOrThrow("channel:moderate", nameof(OnChatModeratorActions));
                s_OnChannelChatModeratorActions += value;
            }
            remove { s_OnChannelChatModeratorActions -= value; }
        }

        public static event Action<Whispers> OnWhispers
        {
            add
            {
                ValidateOrThrow("whispers:read", nameof(OnWhispers));
                s_OnChannelWhispers += value;
            }
            remove { s_OnChannelWhispers -= value; }
        }

        public static void ValidateFromAuthScopes(string[] scopes)
        {
            s_Scopes = scopes;
        }

        public static WebSocketState GetState()
        {
            return s_WebSocket.State;
        }

        public static async Task Start(string authToken, string[] topics)
        {
            try
            {
                InitComponents();

                await ConnectAsync();

                _ = Task.Run(() => ProcessMessageQueue());
                _ = Task.Run(() => Monitor());

                await Heartbeat();
                await Listen(authToken, topics);
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------- Start Exception");
                Console.WriteLine(ex);
            }

        }

        public static async Task ConnectAsync()
        {
            await s_WebSocket.ConnectAsync(new Uri("wss://pubsub-edge.twitch.tv"), s_CancellationSource.Token);
            Console.WriteLine($"WebSocketConnect State: {s_WebSocket.State}");
        }

        public static void InitComponents()
        {
            s_WebSocket = new ClientWebSocket();

            s_MessageQueue = new BlockingCollection<string>();

            s_PingRequest = new PingRequest();

            s_CancellationSource = new CancellationTokenSource();
            s_SendCancellationSource = new CancellationTokenSource();
            s_ProcessCancellationSource = new CancellationTokenSource();
        }

        public static async Task Listen(string authToken, string[] topics)
        {
            ListenRequest listenRequest = new ListenRequest(authToken, topics);
            s_ListenNonce = listenRequest.nonce;
            await SendAsync(JObject.FromObject(listenRequest));

            Console.WriteLine($"Listen to topic(s) {string.Join(",", topics)}");
        }

        public static async Task UnListen(string authToken, string[] topics)
        {
            if (s_WebSocket.State != WebSocketState.Open) return;
            
            await SendAsync(JObject.FromObject(new UnListenRequest(authToken, topics)));

            Console.WriteLine($"UnListen to topic(s) {string.Join(",", topics)}");
        }

        public static async Task Heartbeat()
        {
            s_PongReceived = false;
            
            try
            {
                await SendAsync(JObject.FromObject(s_PingRequest));
                pingSentCounter++;
                Console.WriteLine($"Sent ping {pingSentCounter}");
                    
                // 
                _ = Task.Delay(1000 * 10).ContinueWith(async (task) =>
                {
                    if (s_PongReceived == false)
                    {
                        await s_WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, s_CancellationSource.Token);
                        await InvokeReconnectWithBackoff(ReconnectReason.Handshake);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("------------------- Heartbeat Exception");
                await InvokeReconnectWithBackoff(ReconnectReason.Exception);
            }
        }

        public static async Task Monitor()
        {
            CancellationToken cancellationToken = s_CancellationSource.Token;
            while (cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    Console.WriteLine($"Monitor State: {s_WebSocket.State}");
                    if ((s_WebSocket.State == WebSocketState.Open ||
                        s_WebSocket.State == WebSocketState.CloseSent) && cancellationToken.IsCancellationRequested == false)
                    {
                        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                        WebSocketReceiveResult result = await s_WebSocket.ReceiveAsync(buffer, cancellationToken);

                        if (result == null) continue;

                        if (result.MessageType != WebSocketMessageType.Text)
                        {
                            Thread.Sleep(1);
                            continue;
                        }

                        if (result.EndOfMessage == false)
                        {
                            s_Message += Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                        }
                        else
                        {
                            s_Message += Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                            s_MessageQueue.Add(s_Message);
                            s_Message = string.Empty;
                        }

                    }
                    else if (s_WebSocket.State == WebSocketState.Closed)
                    {
                        await InvokeReconnectWithBackoff(ReconnectReason.Closed);
                        break;
                    }
                    else if (s_WebSocket.State == WebSocketState.Aborted)
                    {
                        await InvokeReconnectWithBackoff(ReconnectReason.Aborted);
                        break;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    OnTaskException?.Invoke(new DebugException(ex, "OperationCanceledException"));
                }
                catch (WebSocketException ex)
                {
                    OnTaskException?.Invoke(new DebugException(ex, "WebSocketException"));
                }
                catch (Exception ex)
                {
                    OnTaskException?.Invoke(new DebugException(ex, "Monitor"));
                    await InvokeReconnectWithBackoff(ReconnectReason.Exception);
                    break;
                }
            }
            await InvokeReconnectWithBackoff(ReconnectReason.OperationCanceled);
        }

        public static void ProcessMessageQueue()
        {
            CancellationToken cancellationToken = s_ProcessCancellationSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (s_MessageQueue.Count == 0)
                {
                    Thread.Sleep(1);
                    continue;
                }

                if (s_MessageQueue.TryTake(out string item))
                {
                    Payload response = JsonConvert.DeserializeObject<Payload>(item);
                    Console.WriteLine($"Process Queue Message: {response.type}");
                    OnPayload?.Invoke(response);

                    switch (response.type)
                    {
                        case "PONG":
                            {
                                s_PongReceived = true;
                                Task.Run(() => DelayedHeartbeat());
                            }
                            break;
                        case "RECONNECT":
                            {
                                OnServerRestart?.Invoke();
                            }
                            break;
                        case "RESPONSE":
                            {
                                PayloadResponse payloadResponse = JsonConvert.DeserializeObject<PayloadResponse>(item);
                                Console.WriteLine($"Process: nonce={payloadResponse.nonce == s_ListenNonce} error={payloadResponse.error}");
                                if(payloadResponse.nonce != s_ListenNonce)
                                {
                                    // Cause fail exception 
                                    // Possible baddie
                                }
                                OnResponse?.Invoke(payloadResponse);
                            }
                            break;
                        case "MESSAGE":
                            {
                                PayloadMessage payloadMessage = JsonConvert.DeserializeObject<PayloadMessage>(item);

                                string topicKey = GetTopicKey(payloadMessage.data.topic);

                                if(topicKey == "whispers")
                                {
                                    JObject jobj = JObject.Parse(payloadMessage.data.message);

                                    JToken jtokenType = jobj.GetValue("type");
                                    JToken jtokenData = jobj.GetValue("data");
                                    string type = jtokenType.ToString();

                                    try
                                    {
                                        Whispers.Data data = JsonConvert.DeserializeObject<Whispers.Data>(jtokenData.ToString());
                                        payloadMessage.data.message = JObject.FromObject(new { type, data }).ToString().Replace("\n", "").Replace("\r", "");
                                        Console.WriteLine($"Process: topic={payloadMessage.data.topic} message={payloadMessage.data.message}");

                                        if (s_TopicEvents.TryGetValue(topicKey, out Action<IPayload> value))
                                        {
                                            value?.Invoke(payloadMessage);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                        OnTaskException?.Invoke(new DebugException(ex, "ProcessMessageQueue Whisper"));
                                    }
                                }
                                else
                                {
                                    if (s_TopicEvents.TryGetValue(topicKey, out Action<IPayload> value))
                                    {
                                        value?.Invoke(payloadMessage);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            _ = InvokeReconnectWithBackoff(ReconnectReason.DefaultBreak);
        }

        static async Task SendAsync(JObject o)
        {
            if (s_WebSocket.State != WebSocketState.Open) return;
            if (s_SendCancellationSource.IsCancellationRequested) return;

            byte[] buffer = Encoding.UTF8.GetBytes(o.ToString());
            await s_WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, s_SendCancellationSource.Token);
        }

        static void ValidateOrThrow(string scope, string name)
        {
            if (s_Scopes != null && s_Scopes.Length > 0)
            {
                if (!s_Scopes.Contains(scope)) throw new InvalidOperationException($"Missing {scope} auth token scope to subscribe to {name} event");
            }
        }

        static T Convert<T>(IPayload payloadMessage) where T : IPayload
        {
            return JsonConvert.DeserializeObject<T>(((PayloadMessage)payloadMessage).data.message);
        }

        static string GetTopicKey(string topic)
        {
            return topic.Substring(0, topic.LastIndexOf('.'));
        }
        
        static async Task InvokeReconnectWithBackoff(ReconnectReason reconnectReason)
        {
            s_CancellationSource.Cancel();
            s_ProcessCancellationSource.Cancel();
            s_SendCancellationSource.Cancel();

            int interval = s_ReconnectInterval + GetJitter(1000);

            Console.WriteLine($"Reconnecting in {interval}  Reason: {reconnectReason}");

            await Task.Delay(interval);
            s_ReconnectInterval *= 2;
            if (s_ReconnectInterval > MAX_BACKOFF_THRESHOLD_INTERVAL)
            {
                s_ReconnectInterval = MAX_BACKOFF_THRESHOLD_INTERVAL;
            }
            OnReconnectRequired?.Invoke(reconnectReason);
        }

        static async void DelayedHeartbeat()
        {
            await Task.Delay(HEARTBEAT_DELAY_TIME_MS + GetJitter(3000));
            await Heartbeat();
        }

        static int GetJitter(int maxMs)
        {
            return s_Rnd.Next(0, maxMs / 1000);
        }

    }
}
