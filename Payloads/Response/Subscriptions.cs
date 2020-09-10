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
    public class Subscriptions : Payload
    {
        // sub/resub message
        public string user_name { get; set; }
        public string display_name { get; set; }
        public string channel_name { get; set; }
        public string user_id { get; set; }
        public string channel_id { get; set; }
        public DateTime time { get; set; }
        public string sub_plan { get; set; }
        public string sub_plan_name { get; set; }
        public int cumulative_months { get; set; }
        public int streak_months { get; set; }
        public string context { get; set; }
        public bool is_gift { get; set; }
        public SubMessage sub_message { get; set; }

        // subgift message
        public int months { get; set; }
        public string recipient_id { get; set; }
        public string recipient_user_name { get; set; }
        public string recipient_display_name { get; set; }

        // multi-month subgift
        public int multi_month_duration { get; set; }

        // anonsubgift - no new fields
        
        [Serializable]
        public class SubMessage
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
