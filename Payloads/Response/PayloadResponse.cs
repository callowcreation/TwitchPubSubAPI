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

using System;

namespace TwitchPubSubAPI.Payloads.Response
{
    /// <summary>
    /// Response from making an topic request
    /// </summary>
    [Serializable]
    public class PayloadResponse : Payload
    {
        /// <summary>
        /// The nonce that was passed in the request, if one was provided there.
        /// </summary>
        public string nonce { get; set; }

        /// <summary>
        /// The error message associated with the request, or an empty string if there is no error.
        /// <para>For Bits and whispers events requests, error responses can be: ERR_BADMESSAGE, ERR_BADAUTH, ERR_SERVER, ERR_BADTOPIC.</para>
        /// </summary>
        public string error { get; set; }
    }
}
