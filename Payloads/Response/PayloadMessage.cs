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
    public class PayloadMessage : Payload
    {
        public Data data;

        [Serializable]
        public class Data
        {
            public string topic { get; set; }
            public string message { get; set; }
        }
    }
}
