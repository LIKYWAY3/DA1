using System.ComponentModel.DataAnnotations;

namespace ASPtestShop.Models.DTO.Order
{
    public class ApplyCouponRequestDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        public string CouponCode { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }

        public List<int>? CartItemIds { get; set; }
    }

    public class ApplyCouponResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal MinOrderAmount { get; set; }
    }
}
