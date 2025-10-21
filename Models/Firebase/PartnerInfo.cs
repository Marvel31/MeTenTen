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
        public object? EncryptedSharedDEK { get; set; }
        
        // JSON 객체에서 실제 암호화된 DEK 값을 추출하는 메서드
        public string? GetEncryptedSharedDEKValue()
        {
            if (EncryptedSharedDEK == null)
                return null;
                
            try
            {
                // 이미 문자열인 경우
                if (EncryptedSharedDEK is string stringValue)
                {
                    // JSON 객체 문자열인지 확인
                    if (stringValue.TrimStart().StartsWith("{"))
                    {
                        var dekData = JsonConvert.DeserializeObject<dynamic>(stringValue);
                        return dekData?.value?.ToString();
                    }
                    
                    // 기존 문자열 형식인 경우
                    return stringValue;
                }
                
                // JSON 객체인 경우 (dynamic 또는 JObject)
                if (EncryptedSharedDEK is Newtonsoft.Json.Linq.JObject jObject)
                {
                    return jObject["value"]?.ToString();
                }
                
                // dynamic 타입인 경우
                var dynamicValue = EncryptedSharedDEK as dynamic;
                if (dynamicValue != null)
                {
                    return dynamicValue.value?.ToString();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PartnerInfo] Error extracting DEK value: {ex.Message}");
                return null;
            }
        }
    }
}
