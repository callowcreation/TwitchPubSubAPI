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
    public class Whispers : Payload
    {
        public string type { get; set; }
        public Data data { get; set; }
        public DataObject data_object { get; set; }

        public class DataObject
        {
            public string message_id { get; set; }
            public int id { get; set; }
            public string thread_id { get; set; }
            public string body { get; set; }
            public int sent_ts { get; set; }
            public int from_id { get; set; }
            public Tags tags { get; set; }
            public Recipient recipient { get; set; }
            public string nonce { get; set; }
        }
        

        [Serializable]
        public class Emote
        {
            public int start { get; set; }
            public int end { get; set; }
            public int id { get; set; }
        }

        [Serializable]
        public class Badge
        {
            public string id { get; set; }
            public string version { get; set; }
        }
        public class Tags
        {
            public string login { get; set; }
            public string display_name { get; set; }
            public string color { get; set; }
            public Emote[] emotes { get; set; }
            public Badge[] badges { get; set; }
        }

        public class Recipient
        {
            public int id { get; set; }
            public string username { get; set; }
            public string display_name { get; set; }
            public string color { get; set; }
            public object profile_image { get; set; }
        }

        public class Data
        {
            public string message_id { get; set; }
            public int id { get; set; }
            public string thread_id { get; set; }
            public string body { get; set; }
            public int sent_ts { get; set; }
            public int from_id { get; set; }
            public Tags tags { get; set; }
            public Recipient recipient { get; set; }
            public string nonce { get; set; }
        }
    }

}
