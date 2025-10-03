using System.ComponentModel.DataAnnotations;

namespace MeTenTenAPI.DTOs
{
    public class TopicDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public bool IsCompleted { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public int DiaryCount { get; set; }
    }

    public class CreateTopicDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public DateTime? ScheduledDate { get; set; }
    }

    public class UpdateTopicDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public DateTime? ScheduledDate { get; set; }
        
        public bool? IsCompleted { get; set; }
    }
}

