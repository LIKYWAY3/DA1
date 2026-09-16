using ASPtestShop.Auth;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [Authorize(AuthenticationSchemes = AdminCookieAuth.Scheme, Roles = "Admin")]
    [Route("api/admin/email-settings")]
    [ApiController]
    public class AdminEmailApiController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public AdminEmailApiController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // GET: api/admin/email-settings
        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _emailService.GetSettingsAsync();

            // Che giấu mật khẩu ứng dụng để bảo mật
            var maskedPassword = string.IsNullOrEmpty(settings.SenderPassword)
                ? ""
                : new string('•', 12);

            return Ok(new
            {
                settings.SenderName,
                settings.SenderEmail,
                SenderPassword = maskedPassword,
                settings.SmtpServer,
                settings.SmtpPort,
                settings.EnableSsl,
                settings.IsConfigured
            });
        }

        // POST: api/admin/email-settings
        [HttpPost]
        public async Task<IActionResult> SaveSettings([FromBody] AdminEmailSettingDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu cấu hình không hợp lệ." });
            }

            var (success, message) = await _emailService.SaveSettingsAsync(dto);

            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new { success = true, message });
        }

        // POST: api/admin/email-settings/test
        [HttpPost("test")]
        public async Task<IActionResult> TestSendEmail([FromBody] TestEmailRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RecipientEmail))
            {
                return BadRequest(new { success = false, message = "Vui lòng nhập địa chỉ email nhận thư thử nghiệm." });
            }

            var (success, message) = await _emailService.TestSendEmailAsync(request.RecipientEmail.Trim());

            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new { success = true, message });
        }
    }
}
