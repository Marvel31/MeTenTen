using System.ComponentModel.DataAnnotations;

namespace MeTenTenMaui.Models
{
    public class CreateTenTenRequest
    {
        [Required(ErrorMessage = "10&10 내용을 입력해주세요.")]
        [MinLength(10, ErrorMessage = "최소 10자 이상 입력해주세요.")]
        public string Content { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "주제를 선택해주세요.")]
        public string TopicId { get; set; } = string.Empty;
    }
}

