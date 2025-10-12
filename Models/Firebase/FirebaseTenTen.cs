namespace MeTenTenMaui.Models.Firebase
{
    public class FirebaseTenTen
    {
        public string? Id { get; set; }
        public string TopicId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string? UpdatedAt { get; set; }
        public bool IsEncrypted { get; set; } = true; // v1.2부터는 기본 true
    }
}

