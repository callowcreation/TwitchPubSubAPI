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
    /// <summary>
    /// When a message for your subscription is published, you will receive a message containing the applicable data.
    /// </summary>
    [Serializable]
    public class PayloadMessage : Payload
    {
        /// <summary>
        /// Wraps the topics and message fields.
        /// </summary>
        public Data data { get; set; }

        /// <summary>
        /// Topic and message fields
        /// </summary>
        [Serializable]
        public class Data
        {
            /// <summary>
            /// The topic that the message pertains to.
            /// </summary>
            public string topic { get; set; }

            /// <summary>
            /// The body of the message. Depending on the type of message, the message body contains different fields; see below.
            /// </summary>
            public string message { get; set; }
        }
    }
}
