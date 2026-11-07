using ASPtestShop.Models.DTO.Refund;

namespace ASPtestShop.Services.Interfaces
{
    /// <summary>
    /// UC-20: Yêu cầu hoàn tiền (Refund Service)
    /// </summary>
    public interface IRefundService
    {
        // Khách hàng gửi yêu cầu hoàn tiền
        Task<RefundResultDto> RequestRefundAsync(string userId, CreateRefundRequestDto dto);

        // Khách hàng xem danh sách yêu cầu hoàn tiền của mình
        Task<List<RefundRequestDto>> GetMyRefundRequestsAsync(string userId);

        // Lấy thông tin yêu cầu hoàn tiền theo OrderId
        Task<RefundRequestDto?> GetRefundRequestByOrderIdAsync(int orderId, string userId);

        // Admin: Xem tất cả yêu cầu hoàn tiền
        Task<List<RefundRequestDto>> GetAllRefundRequestsAsync();

        // Admin: Đếm số lượng yêu cầu hoàn tiền chờ duyệt
        Task<int> GetPendingRefundCountAsync();

        // Admin: Phê duyệt hoặc từ chối hoàn tiền
        Task<RefundResultDto> ProcessRefundAsync(int refundRequestId, string adminUserId, ProcessRefundDto dto);

        // Khách hàng: Gửi lại thông tin tài khoản khi yêu cầu bị từ chối
        Task<RefundResultDto> ResubmitRefundRequestAsync(string userId, ResubmitRefundRequestDto dto);
    }
}
