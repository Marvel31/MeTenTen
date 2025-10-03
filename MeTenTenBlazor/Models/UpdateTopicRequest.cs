using System.ComponentModel.DataAnnotations;

namespace MeTenTenBlazor.Models
{
    public class UpdateTopicRequest
    {
        [Required(ErrorMessage = "주제를 입력해주세요.")]
        [MaxLength(200, ErrorMessage = "주제는 최대 200자까지 입력 가능합니다.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "날짜를 선택해주세요.")]
        public DateTime TopicDate { get; set; } = DateTime.Today;

        public bool IsActive { get; set; } = true;
    }
}
