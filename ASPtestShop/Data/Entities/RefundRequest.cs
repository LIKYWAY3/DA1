using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPtestShop.Data.Entities
{
    /// <summary>
    /// UC-20: Yêu cầu hoàn tiền (Refund Request)
    /// Lưu trữ thông tin khi khách hàng gửi yêu cầu hoàn tiền cho đơn đã thanh toán.
    /// </summary>
    public class RefundRequest : BaseEntity
    {
        public int RefundRequestId { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [MaxLength(100)]
        public string? BankName { get; set; }

        [MaxLength(50)]
        public string? AccountNumber { get; set; }

        [MaxLength(100)]
        public string? AccountHolderName { get; set; }

        [MaxLength(500)]
        public string? AdminNote { get; set; }

        public DateTime? ProcessedAt { get; set; }

        [MaxLength(100)]
        public string? ProcessedBy { get; set; }
    }
}
