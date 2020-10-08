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

namespace TwitchPubSubAPI.Payloads.Response
{
    /// <summary>
    /// Channel points redemption data nad playload
    /// </summary>
    [System.Serializable]
    public class Points : Payload
    {
        /// <summary>
        /// Channel points redemption data
        /// </summary>
        public Data data { get; set; }

        /// <summary>
        /// Channel points redemption data
        /// </summary>
        public class Data
        {
            /// <summary>
            /// Time the pubsub message was sent
            /// </summary>
            public string timestamp { get; set; }

            /// <summary>
            /// Data about the redemption, includes unique id and user that redeemed it
            /// </summary>
            public Redemption redemption { get; set; }
        }

        /// <summary>
        /// Data about the redemption, includes unique id and user that redeemed it
        /// </summary>
        public class Redemption
        {
            /// <summary>
            /// The redemption ID
            /// </summary>
            public string id { get; set; }

            /// <summary>
            /// User name and ID
            /// </summary>
            public User user { get; set; }

            /// <summary>
            /// ID of the channel in which the reward was redeemed.
            /// </summary>
            public string channel_id { get; set; }

            /// <summary>
            /// Timestamp in which a reward was redeemed
            /// </summary>
            public string redeemed_at { get; set; }

            /// <summary>
            /// Data about the reward that was redeemed
            /// </summary>
            public Reward reward { get; set; }

            /// <summary>
            /// reward redemption status, will be FULFULLED if a user skips the reward queue, UNFULFILLED otherwise
            /// </summary>
            public string status { get; set; }

            /// <summary>
            /// User name and ID
            /// </summary>
            public class User
            {
                /// <summary>
                /// User ID
                /// </summary>
                public string id { get; set; }

                /// <summary>
                /// User login name (all lowercase)
                /// </summary>
                public string login { get; set; }

                /// <summary>
                /// User display name
                /// </summary>
                public string display_name { get; set; }
            }

            /// <summary>
            /// Data about the reward that was redeemed
            /// </summary>
            public class Reward
            {
                /// <summary>
                /// Reward id
                /// </summary>
                public string id { get; set; }

                /// <summary>
                /// ID of the channel in which the reward was redeemed.
                /// </summary>
                public string channel_id { get; set; }

                /// <summary>
                /// The rewart title on the card
                /// </summary>
                public string title { get; set; }

                /// <summary>
                /// The discription of the reward
                /// </summary>
                public string prompt { get; set; }

                /// <summary>
                /// Cost in channel points for this reward
                /// </summary>
                public int cost { get; set; }

                /// <summary>
                /// Is the required to send a message along with the reward redemption
                /// </summary>
                public bool is_user_input_required { get; set; }

                /// <summary>
                /// Custom images for the reward
                /// </summary>
                public Image image { get; set; }

                /// <summary>
                /// Default image set
                /// </summary>
                public Image default_image { get; set; }

                /// <summary>
                /// Color of the background selected when created/edited
                /// </summary>
                public string background_color { get; set; }

                /// <summary>
                /// Redemptions allowed per stream
                /// </summary>
                public MaxPerStream max_per_stream { get; set; }

                /// <summary>
                /// Skip the queue that allows the broadcaster to manually accept the reward redemption
                /// </summary>
                public bool should_redemptions_skip_request_queue { get; set; }
                
#pragma warning disable 1591
                public bool is_sub_only { get; set; }

                public bool is_enabled { get; set; }

                public bool is_paused { get; set; }

                public bool is_in_stock { get; set; }

                public string template_id { get; set; }

                public string updated_for_indicator_at { get; set; }
#pragma warning restore 1591

                /// <summary>
                /// Image url for the reward card
                /// </summary>
                public class Image
                {
                    /// <summary>
                    /// Images url size 1x
                    /// </summary>
                    public string url_1x { get; set; }

                    /// <summary>
                    /// Images url size 2x
                    /// </summary>
                    public string url_2x { get; set; }

                    /// <summary>
                    /// Images url size 4x
                    /// </summary>
                    public string url_4x { get; set; }
                }

                /// <summary>
                /// Redemptions allowed per stream
                /// </summary>
                public class MaxPerStream
                {
                    /// <summary>
                    /// Redemption max per stream enabled/disabled
                    /// </summary>
                    public bool is_enabled { get; set; }

                    /// <summary>
                    /// Maximum reward redemptions per stream
                    /// <para>Only used when max per stream is enabled</para>
                    /// </summary>
                    public int max_per_stream { get; set; }
                }
            }
        }
    }
}
