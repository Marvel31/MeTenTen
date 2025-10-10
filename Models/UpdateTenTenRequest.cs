using System.ComponentModel.DataAnnotations;

namespace MeTenTenMaui.Models
{
    public class UpdateTenTenRequest
    {
        [Required(ErrorMessage = "10&10 내용을 입력해주세요.")]
        [MinLength(10, ErrorMessage = "최소 10자 이상 입력해주세요.")]
        public string Content { get; set; } = string.Empty;
    }
}

