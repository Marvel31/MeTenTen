using System.ComponentModel.DataAnnotations;

namespace MeTenTenMaui.Models
{
    public class Prayer
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "제목을 입력해주세요.")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "내용을 입력해주세요.")]
        public string Content { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "카테고리를 선택해주세요.")]
        public string Category { get; set; } = string.Empty;
        
        public string? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsFavorite { get; set; }
        public int ViewCount { get; set; }
    }
}

