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
    [Serializable]
    public class BitsBadge : Payload
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string channel_id { get; set; }
        public string channel_name { get; set; }
        public int badge_tier { get; set; }
        public string chat_message { get; set; }
        public string time { get; set; }
    }
}
