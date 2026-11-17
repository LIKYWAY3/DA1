using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminReviewService : IAdminReviewService
    {
        private readonly AppDbContext _context;

        public AdminReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminReviewDto>> GetReviewsAsync(string? filter = "all")
        {
            var query = _context.Reviews.AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.User)
                .AsQueryable();

            if (filter == "pending")
            {
                query = query.Where(r => !r.IsApproved);
            }
            else if (filter == "approved")
            {
                query = query.Where(r => r.IsApproved);
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new AdminReviewDto
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    ProductName = r.Product.ProductName,
                    ProductThumbnail = r.Product.ThumbnailUrl,
                    UserId = r.UserId,
                    CustomerName = r.User != null ? (r.User.FullName ?? r.User.UserName ?? "Khách hàng") : "Khách hàng",
                    CustomerEmail = r.User != null ? (r.User.Email ?? "") : "",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    IsApproved = r.IsApproved,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> ApproveReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return (false, "Không tìm thấy đánh giá!");

            review.IsApproved = true;
            await _context.SaveChangesAsync();
            return (true, "Đã phê duyệt đánh giá thành công! Đánh giá sẽ hiển thị trên trang sản phẩm.");
        }

        public async Task<(bool Success, string Message)> RejectReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return (false, "Không tìm thấy đánh giá!");

            review.IsApproved = false;
            await _context.SaveChangesAsync();
            return (true, "Đã ẩn đánh giá khỏi trang sản phẩm!");
        }

        public async Task<(bool Success, string Message)> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return (false, "Không tìm thấy đánh giá cần xóa!");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return (true, "Đã xóa đánh giá thành công!");
        }

        public async Task<BatchReviewActionResultDto> BatchApproveReviewsAsync(List<int> reviewIds)
        {
            if (reviewIds == null || !reviewIds.Any())
            {
                return new BatchReviewActionResultDto
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một đánh giá để duyệt."
                };
            }

            var distinctIds = reviewIds.Distinct().ToList();
            var reviews = await _context.Reviews.Where(r => distinctIds.Contains(r.ReviewId)).ToListAsync();

            if (!reviews.Any())
            {
                return new BatchReviewActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đánh giá nào trong danh sách đã chọn."
                };
            }

            foreach (var r in reviews)
            {
                r.IsApproved = true;
            }

            await _context.SaveChangesAsync();

            return new BatchReviewActionResultDto
            {
                Success = true,
                Message = $"Đã duyệt thành công {reviews.Count} đánh giá.",
                SuccessCount = reviews.Count,
                FailedCount = distinctIds.Count - reviews.Count
            };
        }

        public async Task<BatchReviewActionResultDto> BatchRejectReviewsAsync(List<int> reviewIds)
        {
            if (reviewIds == null || !reviewIds.Any())
            {
                return new BatchReviewActionResultDto
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một đánh giá để ẩn."
                };
            }

            var distinctIds = reviewIds.Distinct().ToList();
            var reviews = await _context.Reviews.Where(r => distinctIds.Contains(r.ReviewId)).ToListAsync();

            if (!reviews.Any())
            {
                return new BatchReviewActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đánh giá nào trong danh sách đã chọn."
                };
            }

            foreach (var r in reviews)
            {
                r.IsApproved = false;
            }

            await _context.SaveChangesAsync();

            return new BatchReviewActionResultDto
            {
                Success = true,
                Message = $"Đã ẩn thành công {reviews.Count} đánh giá khỏi website.",
                SuccessCount = reviews.Count,
                FailedCount = distinctIds.Count - reviews.Count
            };
        }

        public async Task<BatchReviewActionResultDto> BatchDeleteReviewsAsync(List<int> reviewIds)
        {
            if (reviewIds == null || !reviewIds.Any())
            {
                return new BatchReviewActionResultDto
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một đánh giá để xóa."
                };
            }

            var distinctIds = reviewIds.Distinct().ToList();
            var reviews = await _context.Reviews.Where(r => distinctIds.Contains(r.ReviewId)).ToListAsync();

            if (!reviews.Any())
            {
                return new BatchReviewActionResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy đánh giá nào trong danh sách đã chọn."
                };
            }

            _context.Reviews.RemoveRange(reviews);
            await _context.SaveChangesAsync();

            return new BatchReviewActionResultDto
            {
                Success = true,
                Message = $"Đã xóa thành công {reviews.Count} đánh giá.",
                SuccessCount = reviews.Count,
                FailedCount = distinctIds.Count - reviews.Count
            };
        }
    }
}
