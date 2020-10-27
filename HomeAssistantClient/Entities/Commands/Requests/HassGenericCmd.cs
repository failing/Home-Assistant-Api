using Newtonsoft.Json;

namespace HomeAssistantClient.Messages
{
    public class HassGenericCmd : HassCmdRequest
    {
        [JsonIgnore]
        public HassCommandType CommandType { get; set; }

        internal override dynamic Type => CommandType;
    }
}