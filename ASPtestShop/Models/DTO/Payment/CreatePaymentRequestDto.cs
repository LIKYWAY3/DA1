namespace ASPtestShop.Models.DTO.Payment
{
    // DTO dùng để gửi thông tin order qua PaymentProvider
    public class CreatePaymentRequestDto
    {
        public int OrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        // URL callback sau khi thanh toán xong (VNPay, MoMo, ZaloPay)
        public string ReturnUrl { get; set; } = string.Empty;

        // IP của người dùng (VNPay yêu cầu)
        public string IpAddress { get; set; } = "127.0.0.1";

        // Mô tả đơn hàng (dùng trong nội dung chuyển khoản)
        public string OrderDescription { get; set; } = string.Empty;
    }
}