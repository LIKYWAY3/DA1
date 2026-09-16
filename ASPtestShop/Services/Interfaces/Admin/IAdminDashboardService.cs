using ASPtestShop.Models.DTO.Admin;

namespace ASPtestShop.Services.Interfaces.Admin
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardStatsAsync();
    }
}
