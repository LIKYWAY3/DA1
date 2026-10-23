using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.Extensions.Configuration;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    /// <summary>
    /// MoMo Payment Provider – UC-18 Thanh toán Online
    /// Tài liệu: https://developers.momo.vn/v3/docs/payment/api/payment-api
    /// API v2 – payWithMethod
    ///
    /// Sau khi có tài khoản merchant thật, cập nhật appsettings.json:
    ///   MoMo:PartnerCode – Mã partner từ MoMo
    ///   MoMo:AccessKey   – Access key từ MoMo
    ///   MoMo:SecretKey   – Secret key để ký HMAC-SHA256
    ///   MoMo:Endpoint    – https://payment.momo.vn (production) hoặc sandbox
    ///   MoMo:ReturnUrl   – URL redirect sau thanh toán
    ///   MoMo:NotifyUrl   – URL IPN MoMo gọi để xác nhận
    /// </summary>
    public class MomoPaymentProvider : IPaymentProvider
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public MomoPaymentProvider(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public string PaymentMethod => "MOMO";

        public async Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var partnerCode = _config["MoMo:PartnerCode"] ?? "";
            var accessKey   = _config["MoMo:AccessKey"]   ?? "";
            var secretKey   = _config["MoMo:SecretKey"]   ?? "";
            var endpoint    = _config["MoMo:Endpoint"]    ?? "https://test-payment.momo.vn/v2/gateway/api/create";
            var returnUrl   = !string.IsNullOrWhiteSpace(request.ReturnUrl)
                ? request.ReturnUrl
                : (_config["MoMo:ReturnUrl"] ?? "/checkout/momo-return");

            var notifyUrl   = _config["MoMo:NotifyUrl"] ?? "/checkout/momo-notify";
            if (!notifyUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                Uri.TryCreate(returnUrl, UriKind.Absolute, out var returnUri))
            {
                notifyUrl = $"{returnUri.Scheme}://{returnUri.Authority}{(notifyUrl.StartsWith('/') ? notifyUrl : "/" + notifyUrl)}";
            }
            var requestType = _config["MoMo:RequestType"] ?? "payWithMethod";

            // Kiểm tra cấu hình
            if (accessKey.StartsWith("CHANGE_ME") || secretKey.StartsWith("CHANGE_ME"))
            {
                return new CreatePaymentResultDto
                {
                    IsSuccess = false,
                    Message   = "MoMo chưa được cấu hình. Vui lòng cập nhật MoMo:AccessKey và MoMo:SecretKey trong appsettings.json.",
                    PaymentMethod = "MOMO",
                    PaymentStatus = "Failed",
                    RequiresRedirect = false
                };
            }

            var requestId   = Guid.NewGuid().ToString("N");
            var orderId     = request.OrderCode; // dùng OrderCode làm orderId để unique
            var amount      = (long)request.Amount;
            var orderInfo   = $"Thanh toan don hang {request.OrderCode}";
            var extraData   = "";
            var lang        = "vi";

            // Tạo chữ ký HMAC-SHA256
            // Thứ tự tham số theo tài liệu MoMo v2
            var rawSignature =
                $"accessKey={accessKey}" +
                $"&amount={amount}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={notifyUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={partnerCode}" +
                $"&redirectUrl={returnUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={requestType}";

            var signature = HmacSha256(secretKey, rawSignature);

            var payload = new
            {
                partnerCode,
                partnerName  = "FuuFishing Store",
                storeId      = partnerCode,
                requestId,
                amount,
                orderId,
                orderInfo,
                redirectUrl  = returnUrl,
                ipnUrl       = notifyUrl,
                lang,
                extraData,
                requestType,
                signature
            };

            try
            {
                var client  = _httpClientFactory.CreateClient();
                var json    = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);
                var body     = await response.Content.ReadAsStringAsync();

                using var doc  = JsonDocument.Parse(body);
                var root        = doc.RootElement;

                var resultCode  = root.TryGetProperty("resultCode", out var rcProp) ? rcProp.GetInt32() : -1;
                var payUrl      = root.TryGetProperty("payUrl", out var puProp) ? puProp.GetString() : null;
                var message     = root.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : "Lỗi MoMo";

                if (resultCode == 0 && !string.IsNullOrEmpty(payUrl))
                {
                    return new CreatePaymentResultDto
                    {
                        IsSuccess        = true,
                        Message          = "Tạo URL thanh toán MoMo thành công",
                        PaymentMethod    = "MOMO",
                        PaymentStatus    = "Pending",
                        PaymentUrl       = payUrl,
                        TransactionCode  = requestId,
                        RequiresRedirect = true
                    };
                }

                return new CreatePaymentResultDto
                {
                    IsSuccess = false,
                    Message   = $"MoMo lỗi (code {resultCode}): {message}",
                    PaymentMethod = "MOMO",
                    PaymentStatus = "Failed",
                    RequiresRedirect = false
                };
            }
            catch (Exception ex)
            {
                return new CreatePaymentResultDto
                {
                    IsSuccess = false,
                    Message   = $"Không thể kết nối MoMo: {ex.Message}",
                    PaymentMethod = "MOMO",
                    PaymentStatus = "Failed",
                    RequiresRedirect = false
                };
            }
        }

        /// <summary>
        /// Xác minh chữ ký khi MoMo IPN/Return callback về server
        /// Gọi trong CheckoutController.MomoReturn() / MomoNotify()
        /// </summary>
        public bool VerifySignature(IDictionary<string, string> callbackParams, string secretKey)
        {
            // Lấy các tham số từ callback theo thứ tự MoMo quy định
            callbackParams.TryGetValue("accessKey",   out var accessKey);
            callbackParams.TryGetValue("amount",      out var amount);
            callbackParams.TryGetValue("extraData",   out var extraData);
            callbackParams.TryGetValue("message",     out var message);
            callbackParams.TryGetValue("orderId",     out var orderId);
            callbackParams.TryGetValue("orderInfo",   out var orderInfo);
            callbackParams.TryGetValue("orderType",   out var orderType);
            callbackParams.TryGetValue("partnerCode", out var partnerCode);
            callbackParams.TryGetValue("payType",     out var payType);
            callbackParams.TryGetValue("requestId",   out var requestId);
            callbackParams.TryGetValue("responseTime",out var responseTime);
            callbackParams.TryGetValue("resultCode",  out var resultCode);
            callbackParams.TryGetValue("transId",     out var transId);
            callbackParams.TryGetValue("signature",   out var signature);

            var rawSignature =
                $"accessKey={accessKey}" +
                $"&amount={amount}" +
                $"&extraData={extraData}" +
                $"&message={message}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&orderType={orderType}" +
                $"&partnerCode={partnerCode}" +
                $"&payType={payType}" +
                $"&requestId={requestId}" +
                $"&responseTime={responseTime}" +
                $"&resultCode={resultCode}" +
                $"&transId={transId}";

            var expected = HmacSha256(secretKey, rawSignature);
            return string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);
        }

        private static string HmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}