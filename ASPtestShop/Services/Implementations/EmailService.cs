using System.Net;
using System.Net.Mail;
using System.Text.Json;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Services.Interfaces;

namespace ASPtestShop.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EmailService> _logger;
        private static AdminEmailSettingDto? _cachedSettings = null;
        private static readonly object _lock = new();

        public EmailService(
            IConfiguration configuration,
            IWebHostEnvironment env,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _env = env;
            _logger = logger;
        }

        private string GetConfigFilePath()
        {
            var dir = Path.Combine(_env.ContentRootPath, "App_Data");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir, "email_settings.json");
        }

        public async Task<AdminEmailSettingDto> GetSettingsAsync()
        {
            lock (_lock)
            {
                if (_cachedSettings != null)
                {
                    return CloneSettings(_cachedSettings);
                }
            }

            var filePath = GetConfigFilePath();
            AdminEmailSettingDto settings = new();

            if (File.Exists(filePath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    var parsed = JsonSerializer.Deserialize<AdminEmailSettingDto>(json);
                    if (parsed != null)
                    {
                        settings = parsed;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi đọc file cấu hình email_settings.json");
                }
            }
            else
            {
                // Đọc fallback từ appsettings.json
                settings.SenderName = _configuration["EmailSettings:SenderName"] ?? "FuuFishing Support";
                settings.SenderEmail = _configuration["EmailSettings:SenderEmail"] ?? "";
                settings.SenderPassword = _configuration["EmailSettings:SenderPassword"] ?? "";
                settings.SmtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
                if (int.TryParse(_configuration["EmailSettings:SmtpPort"], out int port))
                {
                    settings.SmtpPort = port;
                }
            }

            settings.IsConfigured = !string.IsNullOrWhiteSpace(settings.SenderEmail) &&
                                    !string.IsNullOrWhiteSpace(settings.SenderPassword);

            lock (_lock)
            {
                _cachedSettings = CloneSettings(settings);
            }

            return settings;
        }

        public async Task<(bool Success, string Message)> SaveSettingsAsync(AdminEmailSettingDto newSettings)
        {
            if (string.IsNullOrWhiteSpace(newSettings.SenderEmail))
            {
                return (false, "Email người gửi không được để trống.");
            }

            var current = await GetSettingsAsync();

            // Nếu người dùng không nhập lại mật khẩu (để nguyên mask), giữ lại mật khẩu cũ
            if (string.IsNullOrWhiteSpace(newSettings.SenderPassword) || newSettings.SenderPassword.Contains('*'))
            {
                newSettings.SenderPassword = current.SenderPassword;
            }

            newSettings.IsConfigured = !string.IsNullOrWhiteSpace(newSettings.SenderEmail) &&
                                       !string.IsNullOrWhiteSpace(newSettings.SenderPassword);

            try
            {
                var filePath = GetConfigFilePath();
                var json = JsonSerializer.Serialize(newSettings, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);

                lock (_lock)
                {
                    _cachedSettings = CloneSettings(newSettings);
                }

                return (true, "Đã lưu cấu hình Gmail thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lưu cấu hình email");
                return (false, $"Lỗi lưu cấu hình: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var settings = await GetSettingsAsync();

            if (!settings.IsConfigured)
            {
                // Chế độ MOCK / DEV: Ghi log console để lập trình viên và giảng viên có thể kiểm tra mà không bị lỗi
                _logger.LogWarning("==========================================================");
                _logger.LogWarning("[DEV/MOCK EMAIL SENDER] Gmail chưa được cấu hình App Password.");
                _logger.LogWarning($"Gửi đến: {toEmail}");
                _logger.LogWarning($"Tiêu đề: {subject}");
                _logger.LogWarning($"Nội dung: {htmlBody}");
                _logger.LogWarning("==========================================================");

                return (true, "Email đã được mô phỏng gửi thành công (chế độ phát triển).");
            }

            try
            {
                using var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort)
                {
                    EnableSsl = settings.EnableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(
                        settings.SenderEmail.Trim(),
                        settings.SenderPassword.Trim().Replace(" ", "") // App Password thường có dấu cách do Google định dạng
                    ),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 20000 // 20s
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(settings.SenderEmail.Trim(), settings.SenderName.Trim()),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail.Trim());

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"[EMAIL SENT] Gửi thành công tới {toEmail} qua {settings.SenderEmail}");

                return (true, "Đã gửi email thành công!");
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, $"Lỗi SMTP khi gửi email tới {toEmail}: {smtpEx.Message}");
                return (false, $"Lỗi kết nối máy chủ gửi mail Gmail: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi không xác định khi gửi email tới {toEmail}: {ex.Message}");
                return (false, $"Lỗi khi gửi email: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> TestSendEmailAsync(string toEmail)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                return (false, "Vui lòng nhập địa chỉ email nhận thư thử nghiệm.");
            }

            var subject = "[FuuFishing] Thư kiểm tra kết nối Gmail từ Admin Dashboard";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #3b5d50; color: #ffffff; padding: 20px; text-align: center;'>
                        <h2 style='margin: 0;'>FuuFishing - Kiểm tra Email</h2>
                    </div>
                    <div style='padding: 24px; color: #333333; line-height: 1.6;'>
                        <p>Xin chào <strong>Quản trị viên</strong>,</p>
                        <p>Đây là email kiểm thử được gửi từ <strong>Admin Dashboard</strong> của FuuFishing Store.</p>
                        <div style='background-color: #f4fdf8; border-left: 4px solid #28a745; padding: 12px 16px; margin: 20px 0;'>
                            <strong style='color: #28a745;'>✅ Kết nối SMTP Gmail đã hoạt động hoàn hảo!</strong>
                            <p style='margin: 4px 0 0 0; font-size: 14px; color: #555;'>Thời gian gửi: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
                        </div>
                        <p>Hệ thống hiện đã sẵn sàng gửi mã OTP xác thực đăng ký và thông báo đơn hàng cho khách hàng.</p>
                    </div>
                    <div style='background-color: #f8f9fa; padding: 14px; text-align: center; font-size: 12px; color: #888;'>
                        FuuFishing &copy; {DateTime.Now.Year} - Nền tảng Đồ câu chuyên nghiệp
                    </div>
                </div>";

            return await SendEmailAsync(toEmail, subject, body);
        }

        private static AdminEmailSettingDto CloneSettings(AdminEmailSettingDto source)
        {
            return new AdminEmailSettingDto
            {
                SenderName = source.SenderName,
                SenderEmail = source.SenderEmail,
                SenderPassword = source.SenderPassword,
                SmtpServer = source.SmtpServer,
                SmtpPort = source.SmtpPort,
                EnableSsl = source.EnableSsl,
                IsConfigured = source.IsConfigured
            };
        }
    }
}
