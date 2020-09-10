using System;

namespace TwitchPubSubAPI.Payloads
{
    [Serializable]
    public class Payload : IPayload
    {
        public string type { get; set; }
    }
}
