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

using System;

namespace TwitchPubSubAPI.Payloads.Response
{
    /// <summary>
    /// Whisper event received which can be a 'thread' sent before the incoming whisper or message object
    /// <para>Data class only, do not user directly</para>
    /// </summary>
    [Serializable]
    public partial class Whispers : Payload
    {
        /// <summary>
        /// Properties describing the whisper
        /// </summary>
        [System.Obsolete("Use GetData() and GetThreadData()")]
        public Data data { get; set; }

        /// <summary>
        /// Properties describing the whisper
        /// </summary>
        [System.Obsolete("Use GetDataObject() and GetThreadDataObject()")]
        public Data data_object { get; set; }

        /// <summary>
        /// Data for response properties
        /// </summary>
        [Serializable]
        [System.Obsolete("Use WhispersThreadData and WhispersData")]
        public class Data
        {
            // Thread response
            /// <summary>
            /// Thread/Whisper id used to identify the upcoming whisper message
            /// </summary>
            public string id { get; set; } // && Message response

            /// <summary>
            /// Messages between the sender and receiver
            /// </summary>
            public int last_read { get; set; }

            /// <summary>
            /// Message is archived
            /// </summary>
            public bool archived { get; set; }

            /// <summary>
            /// User smeding the message is muted
            /// </summary>
            public bool muted { get; set; }

            /// <summary>
            /// Message is possible spam
            /// </summary>
            public SpamInfo spam_info { get; set; }

            /// <summary>
            /// Time untill the message is not whitlisted
            /// </summary>
            public DateTime whitelisted_until { get; set; }


            // Message response
            /// <summary>
            /// Unique message id
            /// </summary>
            public string message_id { get; set; }

            /*/// <summary>
            /// Messages between the sender and receiver
            /// </summary>
            public int id { get; set; }*/

            /// <summary>
            /// Thread/Whisper id used to identify the upcoming whisper message
            /// </summary>
            public string thread_id { get; set; }

            /// <summary>
            /// The message intended to be read by the recipient
            /// </summary>
            public string body { get; set; }

            /// <summary>
            /// Time the message was sent form the sender
            /// </summary>
            public int sent_ts { get; set; }

            /// <summary>
            /// The senders user ID
            /// </summary>
            public int from_id { get; set; }

            /// <summary>
            /// Senders user display name and color and other properties
            /// </summary>
            public Tags tags { get; set; }

            /// <summary>
            /// Recipient user display name and color and other properties
            /// </summary>
            public Recipient recipient { get; set; }

            /// <summary>
            /// Random string to identify the response associated with this request.
            /// </summary>
            public string nonce { get; set; }
        }

        /// <summary>
        /// Message is possible spam
        /// </summary>
        // Thread response
        public class SpamInfo
        {
            /// <summary>
            /// Liklihood of the following whisper to be flaged as spam
            /// </summary>
            public string likelihood { get; set; }

            /// <summary>
            /// Time last marked as not spam
            /// </summary>
            public int last_marked_not_spam { get; set; }
        }

        /// <summary>
        /// Senders user display name and color and other properties
        /// </summary>
        // Message response
        [Serializable]
        public class Tags
        {
            /// <summary>
            /// User name in all lowecase letters
            /// </summary>
            public string login { get; set; }
            
            /// <summary>
            /// User display name perserves the caseing
            /// </summary>
            public string display_name { get; set; }

            /// <summary>
            /// Color of username in chat
            /// </summary>
            public string color { get; set; }

            /// <summary>
            /// Emote id and positions in message
            /// </summary>
            public Emote[] emotes { get; set; }

            /// <summary>
            /// User badge id and version
            /// </summary>
            public Badge[] badges { get; set; }
        }

        /// <summary>
        /// Recipient user display name and color and other properties
        /// </summary>
        [Serializable]
        public class Recipient
        {
            /// <summary>
            /// User ID
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// User name in all lowecase letters
            /// </summary>
            public string username { get; set; }

            /// <summary>
            /// User display name perserves the caseing
            /// </summary>
            public string display_name { get; set; }

            /// <summary>
            /// Color of username in chat
            /// </summary>
            public string color { get; set; }

            /// <summary>
            /// The url to the users profile image avatar
            /// </summary>
            public object profile_image { get; set; }
        }

        /// <summary>
        /// User badge id and version
        /// </summary>
        [Serializable]
        public class Badge
        {
            /// <summary>
            /// Badge id ie. 'staff'
            /// </summary>
            public string id { get; set; }

            /// <summary>
            ///Badge version number
            /// </summary>
            public string version { get; set; }
        }
    }
    
    /// <summary>
    /// Whisper event data sent
    /// </summary>
    public partial class Whispers
    {
        const string THREAD = "thread";
        const string WHISPER_SENT = "whisper_sent";

        /// <summary>
        /// The type of whisper event, thread or message
        /// </summary>
        public enum ResponseType
        {
            /// <summary>
            /// Whisper thread event is a pre message that may be sent before the whisper message
            /// </summary>
            Thread = 1,

            /// <summary>
            /// The whisper event containing the message
            /// </summary>
            Message = 2
        }

        /// <summary>
        /// The type of whisper event, thread or message
        /// </summary>
        /// <returns>The type of whisper event base on the type json property</returns>
        public ResponseType GetResponseType()
        {
            return type == THREAD ? ResponseType.Thread : ResponseType.Message;
        }

        /// <summary>
        /// Whisper thread event is a pre message data
        /// </summary>
        /// <returns></returns>
        public WhispersThreadData GetThreadData()
        {
            if (type != THREAD) return default(WhispersThreadData);
#pragma warning disable 0618
            return new WhispersThreadData
            {
                id = data.id,
                last_read = data.last_read,
                archived = data.archived,
                muted = data.muted,
                spam_info = data.spam_info,
                whitelisted_until = data.whitelisted_until
            };
#pragma warning restore 0618
        }

        /// <summary>
        /// Whisper thread event is a pre message data
        /// </summary>
        /// <returns></returns>
        public WhispersThreadData GetThreadDataObject()
        {
            if (type != THREAD) return default(WhispersThreadData);
#pragma warning disable 0618
            return new WhispersThreadData
            {
                id = data_object.id,
                last_read = data_object.last_read,
                archived = data_object.archived,
                muted = data_object.muted,
                spam_info = data_object.spam_info,
                whitelisted_until = data_object.whitelisted_until
            };
#pragma warning restore 0618
        }

        /// <summary>
        /// Whisper event is a message data
        /// </summary>
        /// <returns></returns>
        public WhispersData GetData()
        {
            if (type != WHISPER_SENT) return default(WhispersData);
#pragma warning disable 0618
            int.TryParse(data.id, out int value);
            return new WhispersData
            {
                message_id = data.message_id,
                id = value,
                thread_id = data.thread_id,
                body = data.body,
                sent_ts = data.sent_ts,
                from_id = data.from_id,
                tags = data.tags,
                recipient = data.recipient,
                nonce = data.nonce
            };
#pragma warning restore 0618
        }

        /// <summary>
        /// Whisper event is a message data
        /// </summary>
        /// <returns></returns>
        public WhispersData GetDataObject()
        {
            if (type != WHISPER_SENT) return default(WhispersData);
#pragma warning disable 0618
            int.TryParse(data_object.id, out int value);
            return new WhispersData
            {
                message_id = data_object.message_id,
                id = value,
                thread_id = data_object.thread_id,
                body = data_object.body,
                sent_ts = data_object.sent_ts,
                from_id = data_object.from_id,
                tags = data_object.tags,
                recipient = data_object.recipient,
                nonce = data_object.nonce
            };
#pragma warning restore 0618
        }

        /// <summary>
        /// Whisper thread event is a pre message data
        /// </summary>
        [Serializable] // Thread response
        public class WhispersThreadData : Payload
        {
            /// <summary>
            /// Thread id used to identify the upcoming whisper message
            /// </summary>
            public string id { get; set; } // && Message response

            /// <summary>
            /// Messages between the sender and receiver
            /// </summary>
            public int last_read { get; set; }

            /// <summary>
            /// Message is archived
            /// </summary>
            public bool archived { get; set; }

            /// <summary>
            /// User smeding the message is muted
            /// </summary>
            public bool muted { get; set; }

            /// <summary>
            /// Message is possible spam
            /// </summary>
            public SpamInfo spam_info { get; set; }

            /// <summary>
            /// Time untill the message is not whitlisted
            /// </summary>
            public DateTime whitelisted_until { get; set; }
        }

        /// <summary>
        /// Whisper event is a message data
        /// </summary>
        [Serializable]  // Message response
        public class WhispersData : Payload
        {
            /// <summary>
            /// Unique message id
            /// </summary>
            public string message_id { get; set; }

            /// <summary>
            /// Messages between the sender and receiver
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// Thread id used to identify the upcoming whisper message
            /// </summary>
            public string thread_id { get; set; }

            /// <summary>
            /// The message intended to be read by the recipient
            /// </summary>
            public string body { get; set; }

            /// <summary>
            /// Time the message was sent form the sender
            /// </summary>
            public int sent_ts { get; set; }

            /// <summary>
            /// The senders user ID
            /// </summary>
            public int from_id { get; set; }

            /// <summary>
            /// Senders user display name and color and other properties
            /// </summary>
            public Tags tags { get; set; }

            /// <summary>
            /// Recipient user display name and color and other properties
            /// </summary>
            public Recipient recipient { get; set; }

            /// <summary>
            /// Random string to identify the response associated with this request.
            /// </summary>
            public string nonce { get; set; }
        }
    }
}
