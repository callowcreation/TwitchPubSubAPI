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

namespace TwitchPubSubAPI
{
    /// <summary>
    /// Enables notifications from events subscribe to a topic, for updates (e.g., when a user cheers in a channel).
    /// </summary>
    public class PubSubClient
    {
        /// <summary>
        /// Host server
        /// </summary>
        public const string HOST = "wss://pubsub-edge.twitch.tv";

        const int HEARTBEAT_DELAY_TIME_MS = 1000 * 60 * 4;
        const int MAX_BACKOFF_THRESHOLD_INTERVAL = 1000 * 60 * 2;
        const int BACKOFF_THRESHOLD_INTERVAL = 1000 * 3; //ms to wait before reconnect

        const int MAX_PONG_WAIT_INTERVAL = 1000 * 10;

        const string BITS_READ = "bits:read";
        const string CHANNEL_READ_REDEMPTIONS = "channel:read:redemptions";
        const string CHANNEL_SUBSCRIPTIONS = "channel_subscriptions";
        const string WHISPERS_READ = "whispers:read";
        const string CHANNEL_MODERATE = "channel:moderate";

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
            {"whispers", InvokeWhisperReceived },
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
        static event Action<Whispers.WhispersThreadData> s_OnChannelWhispersThread;
        static event Action<Whispers.WhispersData> s_OnChannelWhispers;

        static void InvokeWhisperReceived(IPayload obj)
        {
            Whispers whispers = Convert<Whispers>(obj);

            if (whispers.GetResponseType() == Whispers.ResponseType.Thread)
            {
                s_OnChannelWhispersThread?.Invoke(whispers.GetThreadData());
            }
            else
            {
                s_OnChannelWhispers?.Invoke(whispers.GetData());
            }
        }

        /// <summary>
        /// Sever has sent a reconnect message and we need to reconnect
        /// </summary>
        public static event Action OnServerRestart;

        /// <summary>
        /// Responses received from the server
        /// </summary>
        public static event Action<PayloadResponse> OnResponse;

        /// <summary>
        /// Payload responses received from the server containing data
        /// </summary>
        public static event Action<Payload> OnPayload;

        /// <summary>
        /// Reconnection is attemped and the reason why reconnection is needed
        /// </summary>
        public static event Action<ReconnectReason> OnReconnectRequired;

        /// <summary>
        /// An exception occured within a Task
        /// </summary>
        public static event Action<TaskExceptionArgs> OnTaskException;

        /// <summary>
        /// Cheers happen in a particular channel
        /// </summary>
        public static event Action<Bits> OnBits
        {
            add
            {
                ValidateOrThrow(BITS_READ, nameof(OnBits));
                s_OnChannelBits += value;
            }
            remove { s_OnChannelBits -= value; }
        }

        /// <summary>
        /// A user earns a new Bits badge in a particular channel, and chooses to share the notification with chat.
        /// </summary>
        public static event Action<BitsBadge> OnBitsBadge
        {
            add
            {
                ValidateOrThrow(BITS_READ, nameof(OnBitsBadge));
                s_OnChannelBitsBadge += value;
            }
            remove { s_OnChannelBitsBadge -= value; }
        }

        /// <summary>
        /// A custom reward is redeemed in a channel.
        /// </summary>
        public static event Action<Points> OnPoints
        {
            add
            {
                ValidateOrThrow(CHANNEL_READ_REDEMPTIONS, nameof(OnPoints));
                s_OnChannelPoints += value;
            }
            remove { s_OnChannelPoints -= value; }
        }

        /// <summary>
        /// Anyone subscribes (first month), resubscribes (subsequent months), or gifts a subscription to a channel. 
        /// </summary>
        public static event Action<Subscriptions> OnSubscriptions
        {
            add
            {
                ValidateOrThrow(CHANNEL_SUBSCRIPTIONS, nameof(OnSubscriptions));
                s_OnChannelSubscriptions += value;
            }
            remove { s_OnChannelSubscriptions -= value; }
        }

        /// <summary>
        /// A moderator performs an action in the channel.
        /// </summary>
        public static event Action<ChatModeratorActions> OnChatModeratorActions
        {
            add
            {
                ValidateOrThrow(CHANNEL_MODERATE, nameof(OnChatModeratorActions));
                s_OnChannelChatModeratorActions += value;
            }
            remove { s_OnChannelChatModeratorActions -= value; }
        }

        /// <summary>
        /// Sent before the incoming whisper 
        /// <para>Can be used for validation of whisper</para>
        /// </summary>
        public static event Action<Whispers.WhispersThreadData> OnWhispersThread
        {
            add
            {
                ValidateOrThrow(WHISPERS_READ, nameof(OnWhispersThread));
                s_OnChannelWhispersThread += value;
            }
            remove { s_OnChannelWhispersThread -= value; }
        }

        /// <summary>
        /// Anyone whispers the specified user (the channel owner).
        /// </summary>
        public static event Action<Whispers.WhispersData> OnWhispers
        {
            add
            {
                ValidateOrThrow(WHISPERS_READ, nameof(OnWhispers));
                s_OnChannelWhispers += value;
            }
            remove { s_OnChannelWhispers -= value; }
        }
        
        /// <summary>
        /// Current websocket state
        /// </summary>
        public static WebSocketState GetState
        {
            get { return s_WebSocket.State; }
        }

        /// <summary>
        /// Set the underlying scopes to validate scopes on event invocation
        /// <para>Scope validation will not be checked if this method is not called</para>
        /// </summary>
        /// <param name="scopes">The scopes that are allowed by the current access token</param>
        public static void ValidateFromAuthScopes(string[] scopes)
        {
            s_Scopes = scopes;
        }

        /// <summary>
        /// Initialize the connection and set up the handshake
        /// Topics: (https://dev.twitch.tv/docs/pubsub#topics)
        /// </summary>
        /// <param name="host">Host server to connect to</param>
        /// <param name="accessToken">Access token with the required scopes to listen for the desired topics</param>
        /// <param name="topics">Topics to handle</param>
        /// <returns></returns>
        public static async Task Start(string host, string accessToken, string[] topics)
        {
            try
            {
                InitComponents();

                await ConnectAsync(host);

                _ = Task.Run(() => ProcessMessageQueue());
                _ = Task.Run(() => Monitor());

                await Heartbeat();
                await Listen(accessToken, topics);
            }
            catch (Exception ex)
            {
                OnTaskException?.Invoke(new TaskExceptionArgs(ex, "Startup Failed"));
                throw ex;
            }

        }

        /// <summary>
        /// Connect to the PubSub server
        /// </summary>
        /// <param name="host">Host server to connect to</param>
        /// <returns></returns>
        public static async Task ConnectAsync(string host)
        {
            await s_WebSocket.ConnectAsync(new Uri(host), s_CancellationSource.Token);
        }

        /// <summary>
        /// Initialize websockes and other members
        /// </summary>
        public static void InitComponents()
        {
            s_WebSocket = new ClientWebSocket();

            s_MessageQueue = new BlockingCollection<string>();

            s_PingRequest = new PingRequest();

            s_CancellationSource = new CancellationTokenSource();
            s_SendCancellationSource = new CancellationTokenSource();
            s_ProcessCancellationSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Send a request to listen to multiple topics
        /// </summary>
        /// <param name="accessToken">Access token with the required scopes to listen for the desired topics</param>
        /// <param name="topics">Topics to handle</param>
        /// <returns></returns>
        public static async Task Listen(string accessToken, string[] topics)
        {
            ListenRequest listenRequest = new ListenRequest(accessToken, topics);
            s_ListenNonce = listenRequest.nonce;
            await SendAsync(JObject.FromObject(listenRequest));
        }

        /// <summary>
        /// Send a request to stop/unlisten to multiple topics
        /// </summary>
        /// <param name="accessToken">Access token with the required scopes to listen for the desired topics</param>
        /// <param name="topics">Topics to handle</param>
        /// <returns></returns>
        public static async Task UnListen(string accessToken, string[] topics)
        {
            if (s_WebSocket.State != WebSocketState.Open) return;

            await SendAsync(JObject.FromObject(new UnListenRequest(accessToken, topics)));
        }

        /// <summary>
        /// Maintains the heartbeat PING/PONG to keep the connection open
        /// </summary>
        /// <returns></returns>
        public static async Task Heartbeat()
        {
            s_PongReceived = false;

            try
            {
                await SendAsync(JObject.FromObject(s_PingRequest));
                pingSentCounter++;

                _ = Task.Delay(1000 * 10).ContinueWith(async (task) =>
                {
                    if (s_PongReceived == false)
                    {
                        await s_WebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, s_CancellationSource.Token);
                        await InvokeReconnectWithBackoff(ReconnectReason.Handshake);
                    }
                });
            }
            catch (Exception)
            {
                await InvokeReconnectWithBackoff(ReconnectReason.Exception);
            }
        }

        /// <summary>
        /// Monitor incoming websocket messages and adds them to a message queue
        /// </summary>
        /// <returns></returns>
        public static async Task Monitor()
        {
            CancellationToken cancellationToken = s_CancellationSource.Token;
            while (cancellationToken.IsCancellationRequested == false)
            {
                try
                {
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
                    OnTaskException?.Invoke(new TaskExceptionArgs(ex, "OperationCanceledException"));
                }
                catch (WebSocketException ex)
                {
                    OnTaskException?.Invoke(new TaskExceptionArgs(ex, "WebSocketException"));
                }
                catch (Exception ex)
                {
                    OnTaskException?.Invoke(new TaskExceptionArgs(ex, "Monitor"));
                    await InvokeReconnectWithBackoff(ReconnectReason.Exception);
                    break;
                }
            }
            await InvokeReconnectWithBackoff(ReconnectReason.OperationCanceled);
        }

        /// <summary>
        /// Handle incoming websocket messages that are added to the message queue
        /// </summary>
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
                                if (payloadResponse.nonce != s_ListenNonce)
                                {
                                    throw new AccessViolationException($"The nonce received does not match {s_ListenNonce} Unknown: {payloadResponse.nonce}");
                                }
                                OnResponse?.Invoke(payloadResponse);
                            }
                            break;
                        case "MESSAGE":
                            {
                                PayloadMessage payloadMessage = JsonConvert.DeserializeObject<PayloadMessage>(item);

                                string topicKey = GetTopicKey(payloadMessage.data.topic);

                                if (topicKey == "whispers")
                                {
                                    JObject jobj = JObject.Parse(payloadMessage.data.message);

                                    JToken jtokenType = jobj.GetValue("type");
                                    JToken jtokenData = jobj.GetValue("data");
                                    JToken jtokenDataObject = jobj.GetValue("data_object");
                                    string type = jtokenType.ToString();

                                    try
                                    {

#pragma warning disable 0618
                                        Whispers.Data data = JsonConvert.DeserializeObject<Whispers.Data>(jtokenData.ToString());
                                        Whispers.Data data_object = JsonConvert.DeserializeObject<Whispers.Data>(jtokenDataObject.ToString());
#pragma warning restore 0618
                                        payloadMessage.data.message = JObject.FromObject(new { type, data, data_object }).ToString().Replace("\n", "").Replace("\r", "");

                                        if (s_TopicEvents.TryGetValue(topicKey, out Action<IPayload> value))
                                        {
                                            value?.Invoke(payloadMessage);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        OnTaskException?.Invoke(new TaskExceptionArgs(ex, "ProcessMessageQueue Whisper"));
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
