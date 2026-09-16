using ASPtestShop.Models.DTO.Admin;

namespace ASPtestShop.Services.Interfaces.Admin
{
    public interface IAdminCouponService
    {
        Task<List<AdminCouponDto>> GetCouponsAsync();
        Task<AdminCouponDto?> GetCouponByIdAsync(int id);
        Task<(bool Success, string Message, int? CouponId)> CreateCouponAsync(CreateCouponDto dto);
        Task<(bool Success, string Message)> UpdateCouponAsync(int id, UpdateCouponDto dto);
        Task<(bool Success, string Message, bool NewStatus)> ToggleActiveAsync(int id);
        Task<(bool Success, string Message)> DeleteCouponAsync(int id);
    }
}
