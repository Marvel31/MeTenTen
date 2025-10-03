using System.ComponentModel.DataAnnotations;

namespace MeTenTenBlazor.Models
{
    public class UpdateTopicRequest
    {
        [Required(ErrorMessage = "주제 제목을 입력해주세요.")]
        [MaxLength(200, ErrorMessage = "주제 제목은 최대 200자까지 입력 가능합니다.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "주제 설명은 최대 1000자까지 입력 가능합니다.")]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
