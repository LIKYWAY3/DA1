using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Refund
{
    public class ProcessRefundDto
    {
        [Required(ErrorMessage = "Trạng thái xử lý là bắt buộc (Approved hoặc Rejected)")]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Trạng thái chỉ có thể là 'Approved' hoặc 'Rejected'")]
        public string Status { get; set; } = "Approved";

        [MaxLength(100, ErrorMessage = "Mã giao dịch đối soát tối đa 100 ký tự")]
        public string? TransactionCode { get; set; }

        [MaxLength(500, ErrorMessage = "Ghi chú của Admin tối đa 500 ký tự")]
        public string? AdminNote { get; set; }
    }

    public class RefundResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? RefundRequestId { get; set; }
        public string? Status { get; set; }
    }
}
