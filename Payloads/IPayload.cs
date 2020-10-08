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

namespace TwitchPubSubAPI.Payloads
{
    /// <summary>
    /// Payload with type of message send from the server or client
    /// </summary>
    public interface IPayload
    {
        /// <summary>
        /// Type of message send from the server or client
        /// </summary>
        string type { get; set; }
    }
}
