using Newtonsoft.Json;

namespace MeTenTenMaui.Models.Firebase
{
    public class FirebaseTenTen
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        
        [JsonProperty("topicId")]
        public string TopicId { get; set; } = string.Empty;
        
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;
        
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
        
        [JsonProperty("updatedAt")]
        public string? UpdatedAt { get; set; }
        
        [JsonProperty("isEncrypted")]
        public bool IsEncrypted { get; set; } = true; // v1.2부터는 기본 true
        
        [JsonProperty("encryptionType")]
        public string EncryptionType { get; set; } = "personal"; // "personal" or "shared"
    }
}

