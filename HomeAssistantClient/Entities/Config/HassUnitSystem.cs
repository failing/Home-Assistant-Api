using Newtonsoft.Json;

namespace HomeAssistantClient.Messages
{
    public class HassUnitSystem
    {
        [JsonProperty("length")]
        public string Length { get; set; }

        [JsonProperty("mass")]
        public string Mass { get; set; }

        [JsonProperty("pressure")]
        public string Pressure { get; set; }

        [JsonProperty("temperature")]
        public string Temperature { get; set; }

        [JsonProperty("volume")]
        public string Volume { get; set; }

    }
}
