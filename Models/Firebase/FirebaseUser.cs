using Newtonsoft.Json;

namespace MeTenTenMaui.Models.Firebase
{
    public class FirebaseUser
    {
        [JsonProperty("Email")]
        public string? Email { get; set; }
        
        [JsonProperty("DisplayName")]
        public string? DisplayName { get; set; }
        
        [JsonProperty("EncryptedDEK")]
        public string? EncryptedDEK { get; set; } // 비밀번호로 암호화된 DEK
        
        [JsonProperty("CreatedAt")]
        public string CreatedAt { get; set; } = string.Empty;
        
        [JsonProperty("UpdatedAt")]
        public string? UpdatedAt { get; set; }
        
        [JsonProperty("partner")]
        public PartnerInfo? Partner { get; set; }
    }
}

