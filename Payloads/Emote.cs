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
    /// Emote id and positions in message
    /// </summary>
    [Serializable]
    public class Emote
    {
        /// <summary>
        /// First index of the emote in the message
        /// </summary>
        public int start { get; set; }

        /// <summary>
        /// Last index of the emote in the message
        /// </summary>
        public int end { get; set; }

        /// <summary>
        /// Emote id
        /// </summary>
        public int id { get; set; }
    }
}
