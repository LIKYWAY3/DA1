namespace ASPtestShop.Models.DTO.Order
{
    public class CheckoutResultDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public int? OrderId { get; set; }

        public string? OrderCode { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal FinalAmount { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? PaymentMethod { get; set; }

        // URL chuyển hướng sang cổng thanh toán online (VNPay, MoMo, ZaloPay)
        // null nếu là COD
        public string? PaymentUrl { get; set; }

        // True nếu cần redirect người dùng qua PaymentUrl
        public bool RequiresRedirect { get; set; }
    }
}