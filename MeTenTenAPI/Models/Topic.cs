using System.ComponentModel.DataAnnotations;

namespace MeTenTenAPI.Models
{
    public class Topic
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "주제를 입력해주세요.")]
        [MaxLength(200, ErrorMessage = "주제는 최대 200자까지 입력 가능합니다.")]
        public string Subject { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "날짜를 선택해주세요.")]
        public DateTime TopicDate { get; set; } = DateTime.Today;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // 주제를 생성한 사용자
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        
        // 일기 목록
        public ICollection<Diary> Diaries { get; set; } = new List<Diary>();
    }
}

