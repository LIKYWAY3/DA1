using System;
using System.Threading.Tasks;
using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.Extensions.Configuration;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    /// <summary>
    /// VietQR Payment Provider – Chuẩn mã QR NAPAS 24/7 chuyển khoản ngân hàng tự động
    /// Hỗ trợ tất cả ứng dụng ngân hàng và ví điện tử Việt Nam.
    /// URL định dạng: https://img.vietqr.io/image/{BankId}-{AccountNo}-{Template}.png?amount={Amount}&addInfo={OrderCode}&accountName={AccountName}
    /// </summary>
    public class VietQrPaymentProvider : IPaymentProvider
    {
        private readonly IConfiguration _config;

        public VietQrPaymentProvider(IConfiguration config)
        {
            _config = config;
        }

        public string PaymentMethod => "VIETQR";

        public Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var bankId = _config["VietQR:BankId"] ?? "MB";
            var accountNo = _config["VietQR:AccountNo"] ?? "0987654321";
            var accountName = _config["VietQR:AccountName"] ?? "FUU FISHING STORE";
            var template = _config["VietQR:Template"] ?? "compact2";

            var amount = (long)Math.Round(request.Amount);
            var orderCode = request.OrderCode;

            // Xây dựng URL ảnh mã VietQR chuẩn VietQR.io
            var paymentUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-{template}.png?amount={amount}&addInfo={Uri.EscapeDataString(orderCode)}&accountName={Uri.EscapeDataString(accountName)}";

            var result = new CreatePaymentResultDto
            {
                IsSuccess = true,
                Message = "Tạo mã VietQR chuyển khoản thành công",
                PaymentMethod = "VIETQR",
                PaymentStatus = "Unpaid",
                PaymentUrl = paymentUrl,
                TransactionCode = null,
                RequiresRedirect = false // Khách hàng quét mã QR ngay trên trang hóa đơn / đơn hàng
            };

            return Task.FromResult(result);
        }
    }
}
