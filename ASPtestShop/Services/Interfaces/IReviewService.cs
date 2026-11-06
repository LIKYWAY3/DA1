using ASPtestShop.Models.DTO.Review;

namespace ASPtestShop.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ProductReviewsSummaryDto> GetProductReviewsAsync(int productId);
        Task<(bool Success, string Message, ProductReviewItemDto? Review)> CreateReviewAsync(string userId, CreateReviewDto dto);

        /// <summary>
        /// Kiểm tra user có đủ điều kiện đánh giá sản phẩm không.
        /// </summary>
        Task<ReviewEligibilityDto> CheckEligibilityAsync(string userId, int productId);
    }
}
