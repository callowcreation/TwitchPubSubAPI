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
    [Serializable]
    public class ListenRequest : Payload
    {
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

        public string nonce { get; set; }
        public Data data { get; set; }

        public ListenRequest(string authToken, params string[] topics)
        {
            type = "LISTEN";
            nonce = CalculateNonce(15);
            data = new Data
            {
                auth_token = authToken,
                topics = topics
            };
        }

        [Serializable]
        public class Data
        {
            public string[] topics { get; set; }
            public string auth_token { get; set; }
        }
    }
}
