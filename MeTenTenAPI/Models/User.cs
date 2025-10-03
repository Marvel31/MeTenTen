using System.ComponentModel.DataAnnotations;

namespace MeTenTenAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
        
        // 부부 관계를 위한 파트너 ID
        public int? PartnerId { get; set; }
        public User? Partner { get; set; }
        
        // 일기 목록
        public ICollection<Diary> Diaries { get; set; } = new List<Diary>();
        
        // 주제 목록
        public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    }
}

