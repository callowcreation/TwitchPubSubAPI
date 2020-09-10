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
    public class Commerce : Payload
    {
        public string user_name { get; set; }
        public string display_name { get; set; }
        public string channel_name { get; set; }
        public string user_id { get; set; }
        public string channel_id { get; set; }
        public DateTime time { get; set; }
        public string item_image_url { get; set; }
        public string item_description { get; set; }
        public bool supports_channel { get; set; }
        public PurchaseMessage purchase_message { get; set; }

        [Serializable]
        public class PurchaseMessage
        {
            public string message { get; set; }
            public Emote[] emotes { get; set; }
        }

        [Serializable]
        public class Emote
        {
            public int start { get; set; }
            public int end { get; set; }
            public int id { get; set; }
        }
    }
}
