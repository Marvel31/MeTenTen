using System.ComponentModel.DataAnnotations;

namespace MeTenTenBlazor.Models
{
    public class UpdateTenTenRequest
    {
        [MinLength(10, ErrorMessage = "최소 10자 이상 입력해주세요.")]
        public string? Content { get; set; }
        
        [MaxLength(50, ErrorMessage = "감정 태그는 최대 50자까지 입력 가능합니다.")]
        public string? EmotionTag { get; set; }
        
        [Range(1, 5, ErrorMessage = "중요도는 1-5 사이의 값이어야 합니다.")]
        public int? ImportanceLevel { get; set; }
    }
}
