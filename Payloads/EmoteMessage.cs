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

namespace TwitchPubSubAPI.Payloads
{
    /// <summary>
    /// Chat message sent and emote information
    /// </summary>
    [Serializable]
    public class EmoteMessage
    {
        /// <summary>
        /// Chat message sent by user
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Emotes present in the message
        /// </summary>
        public Emote[] emotes { get; set; }
    }
}
