using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Refund
{
    public class ResubmitRefundRequestDto
    {
        [Required(ErrorMessage = "OrderId là bắt buộc")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ngân hàng")]
        [MaxLength(100, ErrorMessage = "Tên ngân hàng tối đa 100 ký tự")]
        public string BankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản ngân hàng")]
        [MaxLength(50, ErrorMessage = "Số tài khoản tối đa 50 ký tự")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên chủ tài khoản")]
        [MaxLength(100, ErrorMessage = "Tên chủ tài khoản tối đa 100 ký tự")]
        public string AccountHolderName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Lý do tối đa 500 ký tự")]
        public string? Reason { get; set; }
    }
}
