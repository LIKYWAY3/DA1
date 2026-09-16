using ASPtestShop.Models.DTO.Review;

namespace ASPtestShop.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ProductReviewsSummaryDto> GetProductReviewsAsync(int productId);
        Task<(bool Success, string Message, ProductReviewItemDto? Review)> CreateReviewAsync(string userId, CreateReviewDto dto);
    }
}
