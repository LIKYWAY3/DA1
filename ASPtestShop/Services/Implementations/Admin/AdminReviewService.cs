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
    }
}
