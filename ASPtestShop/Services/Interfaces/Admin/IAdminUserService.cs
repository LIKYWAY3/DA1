using ASPtestShop.Models.DTO.Admin;

namespace ASPtestShop.Services.Interfaces.Admin
{
    public interface IAdminUserService
    {
        Task<List<AdminUserDto>> GetUsersAsync(string? search = null);
        Task<(bool Success, string Message, bool IsLockedOut)> ToggleLockUserAsync(string currentAdminId, string targetUserId);
    }
}
