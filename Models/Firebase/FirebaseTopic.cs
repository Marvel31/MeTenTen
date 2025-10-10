namespace MeTenTenMaui.Models.Firebase
{
    public class FirebaseTopic
    {
        public string? Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TopicDate { get; set; } = string.Empty; // ISO 8601 format
        public string CreatedAt { get; set; } = string.Empty;
        public string? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

