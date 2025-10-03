using System.ComponentModel.DataAnnotations;

namespace MeTenTenAPI.Models
{
    public class Diary
    {
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // 일기를 작성한 사용자
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        // 관련 주제
        public int TopicId { get; set; }
        public Topic Topic { get; set; } = null!;
        
        // 읽음 상태 (파트너가 읽었는지)
        public bool IsReadByPartner { get; set; } = false;
        
        public DateTime? ReadByPartnerAt { get; set; }
        
        // 감정 태그 (선택사항)
        [MaxLength(50)]
        public string? EmotionTag { get; set; }
        
        // 중요도 (1-5)
        public int? ImportanceLevel { get; set; }
    }
}

