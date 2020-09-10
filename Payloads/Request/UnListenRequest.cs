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

namespace TwitchPubSubAPI.Payloads.Request
{
    [Serializable]
    public class UnListenRequest : ListenRequest
    {
        public UnListenRequest(string authToken, params string[] topics)
            : base(authToken, topics)
        {
            type = "UNLISTEN";
        }
    }
}
