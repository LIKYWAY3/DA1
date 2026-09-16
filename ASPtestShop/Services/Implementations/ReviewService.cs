using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Review;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ProductReviewsSummaryDto> GetProductReviewsAsync(int productId)
        {
            var reviews = await _context.Reviews.AsNoTracking()
                .Include(r => r.User)
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var totalReviews = reviews.Count;
            var avgRating = totalReviews > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 0;

            var starCounts = new Dictionary<int, int>
            {
                { 5, reviews.Count(r => r.Rating == 5) },
                { 4, reviews.Count(r => r.Rating == 4) },
                { 3, reviews.Count(r => r.Rating == 3) },
                { 2, reviews.Count(r => r.Rating == 2) },
                { 1, reviews.Count(r => r.Rating == 1) }
            };

            var reviewItems = reviews.Select(r => new ProductReviewItemDto
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                UserName = r.User?.UserName ?? "Khách hàng",
                FullName = r.User?.FullName ?? r.User?.UserName ?? "Khách hàng",
                AvatarUrl = r.User?.AvatarUrl,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new ProductReviewsSummaryDto
            {
                ProductId = productId,
                AverageRating = avgRating,
                TotalReviews = totalReviews,
                StarCounts = starCounts,
                Reviews = reviewItems
            };
        }

        public async Task<(bool Success, string Message, ProductReviewItemDto? Review)> CreateReviewAsync(string userId, CreateReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return (false, "Số sao đánh giá phải từ 1 đến 5!", null);
            }

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.ProductId && p.IsActive);
            if (!productExists)
            {
                return (false, "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh!", null);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "Không tìm thấy thông tin tài khoản người dùng!", null);
            }

            // Kiểm tra xem người dùng đã từng đánh giá sản phẩm này chưa
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProductId == dto.ProductId && r.UserId == userId);

            if (existingReview != null)
            {
                // Cập nhật đánh giá cũ
                existingReview.Rating = dto.Rating;
                existingReview.Comment = dto.Comment?.Trim();
                existingReview.IsApproved = true; // Duyệt tự động, admin có thể ẩn nếu vi phạm
                existingReview.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return (true, "Đã cập nhật đánh giá của bạn thành công!", new ProductReviewItemDto
                {
                    ReviewId = existingReview.ReviewId,
                    UserId = user.Id,
                    UserName = user.UserName ?? "Khách hàng",
                    FullName = user.FullName ?? user.UserName ?? "Khách hàng",
                    AvatarUrl = user.AvatarUrl,
                    Rating = existingReview.Rating,
                    Comment = existingReview.Comment,
                    CreatedAt = existingReview.UpdatedAt ?? existingReview.CreatedAt
                });
            }

            // Tạo đánh giá mới
            var review = new Review
            {
                ProductId = dto.ProductId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment?.Trim(),
                IsApproved = true, // Tự động duyệt để khách hàng thấy ngay, admin vẫn có quyền duyệt/ẩn ở trang Admin
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return (true, "Cảm ơn bạn đã gửi đánh giá sản phẩm!", new ProductReviewItemDto
            {
                ReviewId = review.ReviewId,
                UserId = user.Id,
                UserName = user.UserName ?? "Khách hàng",
                FullName = user.FullName ?? user.UserName ?? "Khách hàng",
                AvatarUrl = user.AvatarUrl,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            });
        }
    }
}
