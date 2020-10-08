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
    /// The commerce message event
    /// </summary>
    [Serializable]
    public class Commerce : Payload
    {
        /// <summary>
        /// Wraps the topics and message fields.
        /// </summary>
        public Data data { get; set; }

        /// <summary>
        /// Wraps the topics and message fields
        /// </summary>
        [Serializable]
        public class Data
        {
            /// <summary>
            /// The topic that the message pertains to.
            /// </summary>
            public string topic { get; set; }

            /// <summary>
            /// The body of the message. Depending on the type of message, the message body contains different fields; see below.
            /// </summary>
            public string message { get; set; }
        }

        /// <summary>
        /// The body of the user-entered commerce message.
        /// </summary>
        [Serializable]
        public class Message
        {
            /// <summary>
            /// Login name of the person who subscribed or sent a gift subscription
            /// </summary>
            public string user_name { get; set; }

            /// <summary>
            /// Display name of the person who subscribed or sent a gift subscription
            /// </summary>
            public string display_name { get; set; }

            /// <summary>
            /// Name of the channel that has been subscribed or subgifted
            /// </summary>
            public string channel_name { get; set; }

            /// <summary>
            /// User ID of the person who subscribed or sent a gift subscription
            /// </summary>
            public string user_id { get; set; }

            /// <summary>
            /// ID of the channel that has been subscribed or subgifted
            /// </summary>
            public string channel_id { get; set; }

            /// <summary>
            /// Timestamp for the event
            /// </summary>
            public DateTime time { get; set; }
            
            /// <summary>
            /// Url ti image associated with this event
            /// </summary>
            public string item_image_url { get; set; }

            /// <summary>
            /// User readable/friendly description
            /// </summary>
            public string item_description { get; set; }

            /// <summary>
            /// Commerce event supports channel 
            /// </summary>
            public bool supports_channel { get; set; }

            /// <summary>
            /// Message sent and emote information
            /// </summary>
            public EmoteMessage purchase_message { get; set; }

        }
    }
}
