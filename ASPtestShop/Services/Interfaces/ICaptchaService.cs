namespace ASPtestShop.Services.Interfaces
{
    public interface ICaptchaService
    {
        /// <summary>
        /// Xác minh token CAPTCHA (Turnstile) với Cloudflare API từ server-side.
        /// </summary>
        /// <param name="token">Token gửi từ client (cf-turnstile-response)</param>
        /// <returns>True nếu hợp lệ, False nếu không hợp lệ hoặc hết hạn</returns>
        Task<bool> VerifyAsync(string token);
    }
}
