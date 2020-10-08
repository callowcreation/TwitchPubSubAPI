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

namespace TwitchPubSubAPI
{
    /// <summary>
    /// Exception in a Task will invoke an event with these args
    /// </summary>
    public class TaskExceptionArgs : EventArgs
    {
        /// <summary>
        /// The exception that happened
        /// </summary>
        public readonly Exception exception;

        /// <summary>
        /// The reason the exception happened
        /// </summary>
        public readonly string reason;

        /// <summary>
        /// Create a new task exception with the reason for the exception
        /// </summary>
        /// <param name="exception">The exception that happened</param>
        /// <param name="reason">The reason the exception happened</param>
        public TaskExceptionArgs(Exception exception, string reason)
        {
            this.exception = exception;
            this.reason = reason;
        }
    }
}
