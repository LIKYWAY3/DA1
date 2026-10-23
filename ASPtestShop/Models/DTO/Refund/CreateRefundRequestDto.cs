using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Refund
{
    public class CreateRefundRequestDto
    {
        [Required(ErrorMessage = "OrderId là bắt buộc")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do hoàn tiền")]
        [MaxLength(500, ErrorMessage = "Lý do tối đa 500 ký tự")]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? BankName { get; set; }

        [MaxLength(50)]
        public string? AccountNumber { get; set; }

        [MaxLength(100)]
        public string? AccountHolderName { get; set; }
    }
}
