using System.ComponentModel.DataAnnotations;

namespace MeTenTenMaui.Models
{
    public class CreateTopicRequest
    {
        [Required(ErrorMessage = "주제를 입력해주세요.")]
        [MaxLength(200, ErrorMessage = "주제는 최대 200자까지 입력 가능합니다.")]
        public string Subject { get; set; } = string.Empty;
        
        [MaxLength(1000, ErrorMessage = "설명은 최대 1000자까지 입력 가능합니다.")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "주제 날짜를 선택해주세요.")]
        public DateTime TopicDate { get; set; }
    }
}

