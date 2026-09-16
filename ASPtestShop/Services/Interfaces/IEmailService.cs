using ASPtestShop.Models.DTO.Admin;

namespace ASPtestShop.Services.Interfaces
{
    public interface IEmailService
    {
        Task<(bool Success, string Message)> SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task<AdminEmailSettingDto> GetSettingsAsync();
        Task<(bool Success, string Message)> SaveSettingsAsync(AdminEmailSettingDto settings);
        Task<(bool Success, string Message)> TestSendEmailAsync(string toEmail);
    }
}
