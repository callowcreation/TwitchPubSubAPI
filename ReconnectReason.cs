﻿#region Author
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

namespace TwitchPubSubAPI
{
    /// <summary>
    /// Web socket reconnect attempt reason
    /// </summary>
    public enum ReconnectReason
    {
        /// <summary>
        /// Internal
        /// </summary>
        None,

        /// <summary>
        /// Internal
        /// </summary>
        Exception,

        /// <summary>
        /// Handshake did not receive a PONG response
        /// </summary>
        Handshake,

        /// <summary>
        /// Socket closed
        /// </summary>
        Closed,

        /// <summary>
        /// Aborted for handshake failed or no topic was subscribed to
        /// </summary>
        Aborted,
        
        /// <summary>
        /// Break from loop was invoked from the default switch case 
        /// </summary>
        DefaultBreak,

        /// <summary>
        /// Operation cancled can be call by the system with no ill effects
        /// </summary>
        OperationCanceled
    }
}
