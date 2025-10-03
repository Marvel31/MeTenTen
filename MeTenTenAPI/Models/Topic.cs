using System.ComponentModel.DataAnnotations;

namespace MeTenTenAPI.Models
{
    public class Topic
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ScheduledDate { get; set; }
        
        public bool IsCompleted { get; set; } = false;
        
        // 주제를 생성한 사용자
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        
        // 일기 목록
        public ICollection<Diary> Diaries { get; set; } = new List<Diary>();
    }
}

