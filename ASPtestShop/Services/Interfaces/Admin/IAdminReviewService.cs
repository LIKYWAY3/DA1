using ASPtestShop.Models.DTO.Admin;

namespace ASPtestShop.Services.Interfaces.Admin
{
    public interface IAdminReviewService
    {
        Task<List<AdminReviewDto>> GetReviewsAsync(string? filter = "all");
        Task<(bool Success, string Message)> ApproveReviewAsync(int reviewId);
        Task<(bool Success, string Message)> RejectReviewAsync(int reviewId);
        Task<(bool Success, string Message)> DeleteReviewAsync(int reviewId);
        Task<BatchReviewActionResultDto> BatchApproveReviewsAsync(List<int> reviewIds);
        Task<BatchReviewActionResultDto> BatchRejectReviewsAsync(List<int> reviewIds);
        Task<BatchReviewActionResultDto> BatchDeleteReviewsAsync(List<int> reviewIds);
    }
}
