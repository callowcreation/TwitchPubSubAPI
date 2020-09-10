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
    public class ChatModeratorActions : Payload
    {
        public Data data { get; set; }

        [Serializable]
        public class Data
        {
            public string type { get; set; }
            public string moderation_action { get; set; }
            public string[] args { get; set; }
            public string created_by { get; set; }
            public string created_by_user_id { get; set; }
            public string msg_id { get; set; }
            public string target_user_id { get; set; }
            public string target_user_login { get; set; }
            public bool from_automod { get; set; }
        }
    }
}
