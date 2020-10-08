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
    /// Request fields required to listen for a ping/pong handshake
    /// </summary>
    [Serializable]
    public class PingRequest : Payload
    {
        /// <summary>
        /// Construct a ping request object
        /// </summary>
        public PingRequest()
        {
            type = "PING";
        }
    }
}
