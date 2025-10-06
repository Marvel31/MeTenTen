using System.ComponentModel.DataAnnotations;

namespace MeTenTenMaui.Models
{
    public class Feeling
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "이모지를 선택해주세요.")]
        public string Emoji { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "기분을 선택해주세요.")]
        public string Mood { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "느낌 내용을 입력해주세요.")]
        [MinLength(5, ErrorMessage = "최소 5자 이상 입력해주세요.")]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
