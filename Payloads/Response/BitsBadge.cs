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
    /// User earned a new bit badge
    /// </summary>
    [Serializable]
    public class BitsBadge : Payload
    {
        /// <summary>
        /// ID of user who earned the new Bits badge
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// Login of user who earned the new Bits badge
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// ID of channel where user earned the new Bits badge
        /// </summary>
        public string channel_id { get; set; }

        /// <summary>
        /// Login of channel where user earned the new Bits badge
        /// </summary>
        public string channel_name { get; set; }

        /// <summary>
        /// Value of Bits badge tier that was earned (1000, 10000, etc.)
        /// </summary>
        public int badge_tier { get; set; }

        /// <summary>
        /// [Optional] Custom message included with share
        /// </summary>
        public string chat_message { get; set; }

        /// <summary>
        /// Time when the new Bits badge was earned. RFC 3339 format.
        /// </summary>
        public string time { get; set; }
    }
}
