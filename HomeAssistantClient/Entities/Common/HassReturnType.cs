using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace HomeAssistantClient.Messages
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HassReturnType
    {
        [EnumMember(Value = "result")] Result,
        [EnumMember(Value = "event")] Event,
        [EnumMember(Value = "auth_required")] AuthRequired,
        [EnumMember(Value = "auth_invalid")] AuthInvalid,
        [EnumMember(Value = "auth_ok")] AuthOk,
        [EnumMember(Value = "pong")] Pong
    }
}
