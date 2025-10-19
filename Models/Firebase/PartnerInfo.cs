using Newtonsoft.Json;

namespace MeTenTenMaui.Models.Firebase
{
    public class PartnerInfo
    {
        [JsonProperty("partnerId")]
        public string PartnerId { get; set; } = string.Empty;
        
        [JsonProperty("partnerEmail")]
        public string PartnerEmail { get; set; } = string.Empty;
        
        [JsonProperty("partnerDisplayName")]
        public string PartnerDisplayName { get; set; } = string.Empty;
        
        [JsonProperty("connectedAt")]
        public string ConnectedAt { get; set; } = string.Empty;
        
        [JsonProperty("encryptedSharedDEK")]
        public string? EncryptedSharedDEK { get; set; }
    }
}
