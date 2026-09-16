using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Admin
{
    public class AdminCouponDto
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int UsageLimitTotal { get; set; }
        public int UsageLimitPerCustomer { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired => DateTime.Now > EndDate;
        public bool IsUpcoming => DateTime.Now < StartDate;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCouponDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Mã từ 3 đến 50 ký tự")]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string DiscountType { get; set; } = "Percentage"; // "Percentage" hoặc "FixedAmount"

        [Range(1, 100000000, ErrorMessage = "Giá trị giảm giá không hợp lệ")]
        public decimal DiscountValue { get; set; }

        public decimal MinOrderAmount { get; set; } = 0;

        public decimal? MaxDiscountAmount { get; set; }

        public int UsageLimitTotal { get; set; } = 100;

        public int UsageLimitPerCustomer { get; set; } = 1;

        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);
    }

    public class UpdateCouponDto
    {
        [Required]
        public string DiscountType { get; set; } = "Percentage";

        [Range(1, 100000000, ErrorMessage = "Giá trị giảm giá không hợp lệ")]
        public decimal DiscountValue { get; set; }

        public decimal MinOrderAmount { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        public int UsageLimitTotal { get; set; }

        public int UsageLimitPerCustomer { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
