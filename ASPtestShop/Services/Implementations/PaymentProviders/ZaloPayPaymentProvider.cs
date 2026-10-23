using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ASPtestShop.Models.DTO.Payment;
using ASPtestShop.Services.PaymentProviders;
using Microsoft.Extensions.Configuration;

namespace ASPtestShop.Services.Implementations.PaymentProviders
{
    /// <summary>
    /// ZaloPay Payment Provider – UC-18 Thanh toán Online
    /// Tài liệu: https://docs.zalopay.vn/v2/start/
    ///
    /// Sau khi có tài khoản merchant thật, cập nhật appsettings.json:
    ///   ZaloPay:AppId       – App ID từ ZaloPay
    ///   ZaloPay:Key1        – Key1 để tạo chữ ký
    ///   ZaloPay:Key2        – Key2 để xác minh callback
    ///   ZaloPay:Endpoint    – https://openapi.zalopay.vn/v2/create (production)
    ///   ZaloPay:ReturnUrl   – URL redirect sau thanh toán
    ///   ZaloPay:CallbackUrl – URL IPN ZaloPay gọi để xác nhận
    /// </summary>
    public class ZaloPayPaymentProvider : IPaymentProvider
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public ZaloPayPaymentProvider(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public string PaymentMethod => "ZALOPAY";

        public async Task<CreatePaymentResultDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var appId       = _config["ZaloPay:AppId"]       ?? "2553";
            var key1        = _config["ZaloPay:Key1"]        ?? "";
            var endpoint    = _config["ZaloPay:Endpoint"]    ?? "https://sb-openapi.zalopay.vn/v2/create";
            var returnUrl   = !string.IsNullOrWhiteSpace(request.ReturnUrl)
                ? request.ReturnUrl
                : (_config["ZaloPay:ReturnUrl"] ?? "/checkout/zalopay-return");

            var callbackUrl = _config["ZaloPay:CallbackUrl"] ?? "/checkout/zalopay-callback";
            if (!callbackUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                Uri.TryCreate(returnUrl, UriKind.Absolute, out var returnUri))
            {
                callbackUrl = $"{returnUri.Scheme}://{returnUri.Authority}{(callbackUrl.StartsWith('/') ? callbackUrl : "/" + callbackUrl)}";
            }

            // Kiểm tra cấu hình
            if (key1.StartsWith("CHANGE_ME") || string.IsNullOrWhiteSpace(key1))
            {
                return new CreatePaymentResultDto
                {
                    IsSuccess = false,
                    Message   = "ZaloPay chưa được cấu hình. Vui lòng cập nhật ZaloPay:Key1 và ZaloPay:Key2 trong appsettings.json.",
                    PaymentMethod = "ZALOPAY",
                    PaymentStatus = "Failed",
                    RequiresRedirect = false
                };
            }

            var appTransId  = DateTime.Now.ToString("yyMMdd") + "_" + request.OrderCode;
            var appTime     = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var amount      = (long)request.Amount;
            var description = $"FuuFishing - Thanh toan don hang {request.OrderCode}";

            // Tạo embed_data JSON (chứa redirect_url)
            var embedData = JsonSerializer.Serialize(new { redirecturl = returnUrl });

            // item: mảng sản phẩm đơn giản
            var item = JsonSerializer.Serialize(new[] {
                new { itemid = request.OrderCode, itemname = "Đơn hàng FuuFishing", itemquantity = 1, itemprice = amount }
            });

            // Tạo chữ ký HMAC-SHA256 theo chuẩn ZaloPay
            // mac = HMAC-SHA256(key1, app_id|app_trans_id|app_user|amount|app_time|embed_data|item)
            var rawMac =
                $"{appId}|{appTransId}|{request.UserId}|{amount}|{appTime}|{embedData}|{item}";

            var mac = HmacSha256(key1, rawMac);

            var payload = new Dictionary<string, object>
            {
                { "app_id",       int.Parse(appId) },
                { "app_trans_id", appTransId },
                { "app_user",     request.UserId },
                { "app_time",     appTime },
                { "amount",       amount },
                { "item",         item },
                { "description",  description },
                { "embed_data",   embedData },
                { "bank_code",    "" },         // để trống = cho user chọn
                { "callback_url", callbackUrl },
                { "mac",          mac }
            };

            try
            {
                var client  = _httpClientFactory.CreateClient();
                var json    = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);
                var body     = await response.Content.ReadAsStringAsync();

                using var doc    = JsonDocument.Parse(body);
                var root          = doc.RootElement;
                var returnCode    = root.TryGetProperty("return_code", out var rcProp) ? rcProp.GetInt32() : -1;
                var orderUrl      = root.TryGetProperty("order_url", out var urlProp) ? urlProp.GetString() : null;
                var returnMessage = root.TryGetProperty("return_message", out var msgProp) ? msgProp.GetString() : "Lỗi ZaloPay";

                if (returnCode == 1 && !string.IsNullOrEmpty(orderUrl))
                {
                    return new CreatePaymentResultDto
                    {
                        IsSuccess        = true,
                        Message          = "Tạo URL thanh toán ZaloPay thành công",
                        PaymentMethod    = "ZALOPAY",
                        PaymentStatus    = "Pending",
                        PaymentUrl       = orderUrl,
                        TransactionCode  = appTransId,
                        RequiresRedirect = true
                    };
                }

                return new CreatePaymentResultDto
                {
                    IsSuccess = false,
                    Message   = $"ZaloPay lỗi (code {returnCode}): {returnMessage}",
                    PaymentMethod = "ZALOPAY",
                    PaymentStatus = "Failed",
                    RequiresRedirect = false
                };
            }
            catch (Exception ex)
            {
                return new CreatePaymentResultDto
                {
                    IsSuccess = false,
                    Message   = $"Không thể kết nối ZaloPay: {ex.Message}",
                    PaymentMethod = "ZALOPAY",
                    PaymentStatus = "Failed",
                    RequiresRedirect = false
                };
            }
        }

        /// <summary>
        /// Xác minh chữ ký khi ZaloPay callback về server
        /// Gọi trong CheckoutController.ZaloPayCallback()
        /// </summary>
        public bool VerifyCallback(string data, string mac, string key2)
        {
            var expected = HmacSha256(key2, data);
            return string.Equals(expected, mac, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Xác minh checksum khi ZaloPay redirect về ReturnUrl (browser redirect)
        /// Gọi trong CheckoutController.ZaloPayReturn()
        /// </summary>
        public bool VerifyReturn(IDictionary<string, string> queryParams, string key2)
        {
            queryParams.TryGetValue("appid", out var appId);
            queryParams.TryGetValue("apptransid", out var appTransId);
            queryParams.TryGetValue("pmcid", out var pmcId);
            queryParams.TryGetValue("bankcode", out var bankCode);
            queryParams.TryGetValue("amount", out var amount);
            queryParams.TryGetValue("discountamount", out var discountAmount);
            queryParams.TryGetValue("status", out var status);
            queryParams.TryGetValue("checksum", out var checksum);

            if (string.IsNullOrEmpty(checksum) || string.IsNullOrEmpty(key2)) return false;

            var rawData = $"{appId}|{appTransId}|{pmcId}|{bankCode}|{amount}|{discountAmount}|{status}";
            var expected = HmacSha256(key2, rawData);
            return string.Equals(expected, checksum, StringComparison.OrdinalIgnoreCase);
        }

        private static string HmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}