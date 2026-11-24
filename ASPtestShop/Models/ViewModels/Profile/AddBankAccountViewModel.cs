using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.ViewModels.Profile
{
    public class AddBankAccountViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn hoặc nhập ngân hàng")]
        public string BankName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản")]
        [RegularExpression(@"^[0-9]{6,20}$", ErrorMessage = "Số tài khoản ngân hàng chỉ được chứa các chữ số (từ 6 đến 20 số).")]
        public string AccountNumber { get; set; } = null!;

        public string? Branch { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}