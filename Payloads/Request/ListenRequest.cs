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
using System.Security.Cryptography;

namespace TwitchPubSubAPI.Payloads.Request
{
    /// <summary>
    /// Request fields required to listen to a topic
    /// </summary>
    [Serializable]
    public class ListenRequest : Payload
    {
        /// <summary>
        /// Random string to identify the response associated with this request.
        /// </summary>
        public string nonce { get; set; }

        /// <summary>
        /// Access token and topics
        /// </summary>
        public Data data { get; set; }

        /// <summary>
        /// Construct a listen to topic(s) request
        /// </summary>
        /// <param name="accessToken">Access token with the required scope</param>
        /// <param name="topics">Topics to subscribe to</param>
        public ListenRequest(string accessToken, params string[] topics)
        {
            type = "LISTEN";
            nonce = CalculateNonce(15);
            data = new Data
            {
                auth_token = accessToken,
                topics = topics
            };
        }

        /// <summary>
        /// Access token and topics
        /// </summary>
        [Serializable]
        public class Data
        {
            /// <summary>
            /// The topics to subscribe to
            /// </summary>
            public string[] topics { get; set; }

            /// <summary>
            /// Access token with valid scopes for the topics
            /// </summary>
            public string auth_token { get; set; }
        }

        // https://sqlsteve.wordpress.com/2014/04/23/how-to-create-a-nonce-in-c/
        static string CalculateNonce(int length)
        {
            //Allocate a buffer
            var ByteArray = new byte[length];
            //Generate a cryptographically random set of bytes
            using (var Rnd = RandomNumberGenerator.Create())
            {
                Rnd.GetBytes(ByteArray);
            }
            //Base64 encode and then return
            return Convert.ToBase64String(ByteArray);
        }
    }
}
