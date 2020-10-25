using Newtonsoft.Json;

namespace HomeAssistantApi.Messages

{
    public class GenericCommand : CommandRequest
    {
        [JsonIgnore]
        public HassCommandType CommandType { get; set; }

        internal override dynamic Type => CommandType;
    }
}