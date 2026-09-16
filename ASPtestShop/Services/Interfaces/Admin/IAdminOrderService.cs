using ASPtestShop.Models.DTO.Order;

namespace ASPtestShop.Services.Interfaces.Admin
{
    public interface IAdminOrderService
    {
        Task<List<AdminOrderListItemDto>> GetAllOrdersAsync();

        Task<AdminOrderDetailDto?> GetOrderByIdAsync(int orderId);

        Task<AdminOrderActionResultDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<AdminOrderActionResultDto> DeleteCancelledOrderAsync(int orderId);
        Task<AdminOrderActionResultDto> CleanupAllCancelledOrdersAsync();
    }
}