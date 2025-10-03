using System.ComponentModel.DataAnnotations;

namespace MeTenTenAPI.DTOs
{
    public class DiaryDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TopicId { get; set; }
        public string TopicTitle { get; set; } = string.Empty;
        public bool IsReadByPartner { get; set; }
        public DateTime? ReadByPartnerAt { get; set; }
        public string? EmotionTag { get; set; }
        public int? ImportanceLevel { get; set; }
    }

    public class CreateDiaryDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public int TopicId { get; set; }
        
        [MaxLength(50)]
        public string? EmotionTag { get; set; }
        
        [Range(1, 5)]
        public int? ImportanceLevel { get; set; }
    }

    public class UpdateDiaryDto
    {
        public string? Content { get; set; }
        
        [MaxLength(50)]
        public string? EmotionTag { get; set; }
        
        [Range(1, 5)]
        public int? ImportanceLevel { get; set; }
    }

    public class MarkAsReadDto
    {
        [Required]
        public int DiaryId { get; set; }
    }
}

