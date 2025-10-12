namespace MeTenTenMaui.Models
{
    public class TenTen
    {
        public int Id { get; set; }
        public string? FirebaseKey { get; set; } // Firebase Realtime DB key
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TopicId { get; set; }
        public string TopicSubject { get; set; } = string.Empty;
        public bool IsReadByPartner { get; set; }
        public DateTime? ReadByPartnerAt { get; set; }
        public bool IsEncrypted { get; set; } = true; // v1.2부터는 기본 true
    }
}