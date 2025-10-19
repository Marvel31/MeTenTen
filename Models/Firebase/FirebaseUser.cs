using Newtonsoft.Json;

namespace MeTenTenMaui.Models.Firebase
{
    public class FirebaseUser
    {
        [JsonProperty("email")]
        public string? Email { get; set; }
        
        [JsonProperty("displayName")]
        public string? DisplayName { get; set; }
        
        [JsonProperty("encryptedDEK")]
        public string? EncryptedDEK { get; set; } // 비밀번호로 암호화된 DEK
        
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
        
        [JsonProperty("updatedAt")]
        public string? UpdatedAt { get; set; }
        
        [JsonProperty("partner")]
        public PartnerInfo? Partner { get; set; }
    }
}

