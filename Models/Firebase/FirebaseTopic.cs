using Newtonsoft.Json;

namespace MeTenTenMaui.Models.Firebase
{
    public class FirebaseTopic
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        
        [JsonProperty("subject")]
        public string Subject { get; set; } = string.Empty;
        
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
        
        [JsonProperty("topicDate")]
        public string TopicDate { get; set; } = string.Empty; // ISO 8601 format
        
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
        
        [JsonProperty("updatedAt")]
        public string? UpdatedAt { get; set; }
        
        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true;
    }
}

