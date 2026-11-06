using System.Text.Json;

namespace ASPtestShop.Services.Implementations
{
    /// <summary>
    /// Xác minh Cloudflare Turnstile CAPTCHA bằng cách gọi API server-side.
    /// Secret Key chỉ tồn tại ở Backend, không bao giờ gửi ra Client.
    /// </summary>
    public class CloudflareTurnstileService : Services.Interfaces.ICaptchaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CloudflareTurnstileService> _logger;

        private const string VerifyEndpoint = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

        public CloudflareTurnstileService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<CloudflareTurnstileService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> VerifyAsync(string token)
        {
            // Token rỗng hoặc null → từ chối ngay
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("[CAPTCHA] Token rỗng — từ chối xác minh.");
                return false;
            }

            var secretKey = _configuration["Captcha:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.StartsWith("YOUR_"))
            {
                // Fallback testing secret key nếu chưa cấu hình
                secretKey = "1x0000000000000000000000000000000AA";
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", secretKey),
                    new KeyValuePair<string, string>("response", token)
                });

                var response = await client.PostAsync(VerifyEndpoint, formContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("[CAPTCHA] Cloudflare API trả về HTTP {StatusCode}.", response.StatusCode);
                    return false;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var success = doc.RootElement.TryGetProperty("success", out var successProp)
                    && successProp.GetBoolean();

                if (!success)
                {
                    var errorCodes = doc.RootElement.TryGetProperty("error-codes", out var errProp)
                        ? string.Join(", ", errProp.EnumerateArray().Select(e => e.GetString()))
                        : "unknown";

                    _logger.LogWarning("[CAPTCHA] Xác minh thất bại. Lỗi: {Errors}", errorCodes);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CAPTCHA] Lỗi khi gọi Cloudflare Turnstile API.");
                return false;
            }
        }
    }
}
