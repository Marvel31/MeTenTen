namespace MeTenTenBlazor.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public DateTime TopicDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
    }
}
