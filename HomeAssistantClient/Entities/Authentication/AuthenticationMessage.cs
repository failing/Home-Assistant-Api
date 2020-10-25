using Newtonsoft.Json;

namespace HomeAssistantApi.Messages

{
    public class AuthenticationMessage : HassMessage
    {
        internal override dynamic Type => HassCommandType.Auth;

        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}
