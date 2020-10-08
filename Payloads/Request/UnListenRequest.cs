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
    /// <summary>
    /// Request fields required to stop/unlisten from a topic
    /// </summary>
    [Serializable]
    public class UnListenRequest : ListenRequest
    {
        /// <summary>
        /// Construct an unlisten from topic(s) request
        /// </summary>
        /// <param name="accessToken">Access token with the required scope</param>
        /// <param name="topics">Topics to subscribe to</param>
        public UnListenRequest(string accessToken, params string[] topics)
            : base(accessToken, topics)
        {
            type = "UNLISTEN";
        }
    }
}
