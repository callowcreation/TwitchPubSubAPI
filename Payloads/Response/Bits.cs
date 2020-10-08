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
    /// Bits event payload data
    /// </summary>
    [Serializable]
    public class Bits : Payload
    {
        /// <summary>
        /// Bits event data
        /// </summary>
        public Data data { get; set; }

        /// <summary>
        /// Message version
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// Message type (that is, the type of object contained in the data field)
        /// </summary>
        public string message_type { get; set; }

        /// <summary>
        /// Message ID
        /// </summary>
        public string message_id { get; set; }

        /// <summary>
        /// Whether or not the event was anonymous.
        /// </summary>
        public bool is_anonymous { get; set; }

        /// <summary>
        /// Bits event data
        /// </summary>
        [Serializable]
        public class Data
        {
            /// <summary>
            /// Login name of the person who used the Bits - if the cheer was not anonymous.
            /// <para>Null if anonymous</para>
            /// </summary>
            public string user_name { get; set; }

            /// <summary>
            /// Username/broadcaster of the channel in which Bits were used.
            /// </summary>
            public string channel_name { get; set; }

            /// <summary>
            /// User ID of the person who used the Bits - if the cheer was not anonymous.
            /// <para>Null if anonymous</para>
            /// </summary>
            public string user_id { get; set; }

            /// <summary>
            /// ID of the channel in which Bits were used.
            /// </summary>
            public string channel_id { get; set; }

            /// <summary>
            /// Represents an instant in time, typically expressed as a date and time of day.
            /// </summary>
            public DateTime time { get; set; }

            /// <summary>
            /// Chat message sent with the cheer.
            /// </summary>
            public string chat_message { get; set; }

            /// <summary>
            /// Number of Bits used.
            /// </summary>
            public int bits_used { get; set; }

            /// <summary>
            /// All-time total number of Bits used on this channel by the specified user.
            /// </summary>
            public int total_bits_used { get; set; }

            /// <summary>
            /// Event type associated with this use of Bits (for example, cheer).
            /// </summary>
            public string context { get; set; }

            /// <summary>
            /// Information about a user’s new badge level
            /// </summary>
            public BadgeEntitlement badge_entitlement { get; set; }
        }

        /// <summary>
        /// Information about a user’s new badge level
        /// </summary>
        [Serializable]
        public class BadgeEntitlement
        {
            /// <summary>
            /// New bits badge level earned
            /// </summary>
            public int new_version { get; set; }

            /// <summary>
            /// Prevoius bits badge level earned
            /// </summary>
            public int previous_version { get; set; }
        }
    }
}
