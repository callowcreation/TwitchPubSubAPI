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
    /// When a message for your subscription is published, you will receive a message containing the applicable data.
    /// </summary>
    [Serializable]
    public class Subscriptions : Payload
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
        /// The body of the user-entered resub message. Depending on the type of message, the message body contains different fields
        /// </summary>
        [Serializable]
        public class Message
        {
            // sub/resub message
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
            /// Time when the subscription or gift was completed. RFC 3339 format
            /// </summary>
            public DateTime time { get; set; }

            /// <summary>
            /// Subscription Plan ID
            /// <para>values: Prime, 1000, 2000, 3000</para>
            /// </summary>
            public string sub_plan { get; set; }

            /// <summary>
            /// Channel Specific Subscription Plan Name
            /// </summary>
            public string sub_plan_name { get; set; }

            /// <summary>
            /// Cumulative number of tenure months of the subscription
            /// </summary>
            public int cumulative_months { get; set; }

            /// <summary>
            /// Denotes the user’s most recent (and contiguous) subscription tenure streak in the channel
            /// </summary>
            public int streak_months { get; set; }

            /// <summary>
            /// Event type associated with the subscription product
            /// <para>values: sub, resub, subgift, anonsubgift, resubgift, anonresubgift</para>
            /// </summary>
            public string context { get; set; }

            /// <summary>
            /// If this sub message was caused by a gift subscription
            /// </summary>
            public bool is_gift { get; set; }

            /// <summary>
            /// Chat message sent and emote information
            /// </summary>
            public EmoteMessage sub_message { get; set; }

            // subgift message
            /// <summary>
            /// Cumulative number of months the gifter has giften in the channel (Deprecated)
            /// </summary>
            [System.Obsolete("", true)]
            public int months { get; set; }

            /// <summary>
            /// User ID of the subscription gift recipient
            /// </summary>
            public string recipient_id { get; set; }

            /// <summary>
            /// Login name of the subscription gift recipient
            /// </summary>
            public string recipient_user_name { get; set; }

            /// <summary>
            /// Display name of the person who received the subscription gift
            /// </summary>
            public string recipient_display_name { get; set; }

            // multi-month subgift
            /// <summary>
            /// Number of months gifted as part of a single, multi-month gift
            /// </summary>
            public int multi_month_duration { get; set; }

            // anonsubgift - no new fields

        }
    }
}
