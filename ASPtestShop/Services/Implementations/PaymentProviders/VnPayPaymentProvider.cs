using System.Security.Cryptography;
using System.Text;
using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.Extensions.Configuration;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    /// <summary>
    /// VNPay Payment Provider – UC-18 Thanh toán Online
    /// Tài liệu: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
    /// 
    /// Sau khi có tài khoản merchant thật, cập nhật các giá trị trong appsettings.json:
    ///   VnPay:TmnCode      – Mã website tại VNPay
    ///   VnPay:HashSecret   – Chuỗi bí mật HMAC-SHA512
    ///   VnPay:BaseUrl      – URL sandbox hoặc production
    ///   VnPay:ReturnUrl    – URL callback trên server của bạn
    /// </summary>
    public class VnPayPaymentProvider : IPaymentProvider
    {
        private readonly IConfiguration _config;

        public VnPayPaymentProvider(IConfiguration config)
        {
            _config = config;
        }

        public string PaymentMethod => "VNPAY";

        public Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var tmnCode   = _config["VnPay:TmnCode"]   ?? "";
            var hashSecret = _config["VnPay:HashSecret"] ?? "";
            var baseUrl   = _config["VnPay:BaseUrl"]   ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            var returnUrl = _config["VnPay:ReturnUrl"]  ?? "/checkout/vnpay-return";
            var version   = _config["VnPay:Version"]   ?? "2.1.0";
            var command   = _config["VnPay:Command"]   ?? "pay";
            var currCode  = _config["VnPay:CurrCode"]  ?? "VND";
            var locale    = _config["VnPay:Locale"]    ?? "vn";

            // Nếu chưa cấu hình TmnCode hoặc HashSecret thật thì báo lỗi rõ ràng
            if (string.IsNullOrWhiteSpace(tmnCode) || tmnCode.StartsWith("CHANGE_ME") ||
                string.IsNullOrWhiteSpace(hashSecret) || hashSecret.StartsWith("CHANGE_ME"))
            {
                return Task.FromResult(new CreatePaymentResultDto
                {
                    IsSuccess = false,
                    Message   = "VNPay chưa được cấu hình. Vui lòng cập nhật VnPay:TmnCode và VnPay:HashSecret trong appsettings.json.",
                    PaymentMethod = "VNPAY",
                    PaymentStatus = "Failed",
                    RequiresRedirect = false
                });
            }

            // Số tiền VNPay yêu cầu nhân 100 (không có dấu thập phân)
            var vnpAmount = ((long)(request.Amount * 100)).ToString();
            var createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var txnRef = request.OrderCode; // Mã giao dịch của merchant (unique)

            // Xây dựng tập tham số theo thứ tự ABC (VNPay yêu cầu sort)
            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Version",    version },
                { "vnp_Command",    command },
                { "vnp_TmnCode",    tmnCode },
                { "vnp_Amount",     vnpAmount },
                { "vnp_CreateDate", createDate },
                { "vnp_CurrCode",   currCode },
                { "vnp_IpAddr",     request.IpAddress },
                { "vnp_Locale",     locale },
                { "vnp_OrderInfo",  $"Thanh toan don hang {request.OrderCode}" },
                { "vnp_OrderType",  "other" },
                { "vnp_ReturnUrl",  request.ReturnUrl.StartsWith("http") ? request.ReturnUrl : $"{{BaseUrl}}{request.ReturnUrl}" },
                { "vnp_TxnRef",     txnRef }
            };

            // Tạo query string và chữ ký HMAC-SHA512
            var rawData = BuildRawData(vnpParams);
            var signature = HmacSha512(hashSecret, rawData);

            var paymentUrl = $"{baseUrl}?{rawData}&vnp_SecureHash={signature}";

            return Task.FromResult(new CreatePaymentResultDto
            {
                IsSuccess        = true,
                Message          = "Tạo URL thanh toán VNPay thành công",
                PaymentMethod    = "VNPAY",
                PaymentStatus    = "Pending",
                PaymentUrl       = paymentUrl,
                TransactionCode  = null,  // Sẽ được cập nhật sau khi VNPay callback
                RequiresRedirect = true
            });
        }

        /// <summary>
        /// Xác minh chữ ký khi VNPay callback về server
        /// Gọi hàm này trong CheckoutController.VnPayReturn()
        /// </summary>
        public bool VerifySignature(IDictionary<string, string> vnpParams, string secretKey)
        {
            if (!vnpParams.TryGetValue("vnp_SecureHash", out var secureHash))
                return false;

            // Loại bỏ vnp_SecureHash và vnp_SecureHashType khỏi params trước khi ký
            var paramsCopy = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var kv in vnpParams)
            {
                if (!kv.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase) &&
                    !kv.Key.Equals("vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
                {
                    paramsCopy[kv.Key] = kv.Value;
                }
            }

            var rawData   = BuildRawData(paramsCopy);
            var signature = HmacSha512(secretKey, rawData);

            return string.Equals(signature, secureHash, StringComparison.OrdinalIgnoreCase);
        }

        // ── Private helpers ────────────────────────────────────────────────────────

        private static string BuildRawData(SortedDictionary<string, string> paramDict)
        {
            var sb = new StringBuilder();
            foreach (var kv in paramDict)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    if (sb.Length > 0) sb.Append('&');
                    sb.Append(Uri.EscapeDataString(kv.Key));
                    sb.Append('=');
                    sb.Append(Uri.EscapeDataString(kv.Value));
                }
            }
            return sb.ToString();
        }

        private static string HmacSha512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}