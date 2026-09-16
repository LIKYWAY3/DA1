using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.ViewModels.Auth
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Vui lòng cung cấp email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mã xác thực OTP")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã xác thực bao gồm đúng 6 chữ số")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Mã xác thực chỉ bao gồm 6 chữ số")]
        public string OtpCode { get; set; } = string.Empty;
    }
}
