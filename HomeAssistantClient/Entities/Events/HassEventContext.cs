using Newtonsoft.Json;

namespace HomeAssistantClient.Messages
{
    public class HassEventContext
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}