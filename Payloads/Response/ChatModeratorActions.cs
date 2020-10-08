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
    /// A moderator performs an action in the channel. 
    /// </summary>
    [Serializable]
    public class ChatModeratorActions : Payload
    {
        /// <summary>
        /// Information about the action
        /// </summary>
        public Data data { get; set; }

        /// <summary>
        /// Information about the action
        /// </summary>
        [Serializable]
        public class Data
        {

            /// <summary>
            /// Type of moderation action ie. "chat_login_moderation"
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// The moderation action ie. "ban" "unban"
            /// </summary>
            public string moderation_action { get; set; }

            /// <summary>
            /// Contains the target user name and message
            /// </summary>
            public string[] args { get; set; }

            /// <summary>
            /// The user name that created/preformed the action
            /// </summary>
            public string created_by { get; set; }

            /// <summary>
            /// The user ID that created/preformed the action
            /// </summary>
            public string created_by_user_id { get; set; }

            /// <summary>
            /// Id of the message (may be empty)
            /// </summary>
            public string msg_id { get; set; }

            /// <summary>
            /// Target user ID
            /// </summary>
            public string target_user_id { get; set; }

            /// <summary>
            /// Target user name (may be empty user name in 'args')
            /// </summary>
            public string target_user_login { get; set; }

            /// <summary>
            /// Did automod preform the action
            /// </summary>
            public bool from_automod { get; set; }
        }
    }
}
