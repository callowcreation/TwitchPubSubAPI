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
    public class Bits : Payload
    {
        public Data data { get; set; }
        public string version { get; set; }
        public string message_type { get; set; }
        public string message_id { get; set; }
        public bool is_anonymous { get; set; }

        [Serializable]
        public class Data
        {
            public string user_name { get; set; }
            public string channel_name { get; set; }
            public string user_id { get; set; }
            public string channel_id { get; set; }
            public DateTime time { get; set; }
            public string chat_message { get; set; }
            public int bits_used { get; set; }
            public int total_bits_used { get; set; }
            public string context { get; set; }
            public BadgeEntitlement badge_entitlement { get; set; }
        }

        [Serializable]
        public class BadgeEntitlement
        {
            public int new_version { get; set; }
            public int previous_version { get; set; }
        }
    }
}
