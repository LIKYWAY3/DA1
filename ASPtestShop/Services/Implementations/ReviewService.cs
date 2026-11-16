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

            // BẢO MẬT & NGHIỆP VỤ: Đếm số lần mua hoàn tất và số đánh giá đã tạo (1 lần mua hoàn tất = 1 lần đánh giá)
            var completedOrderItems = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.ProductId == dto.ProductId &&
                             oi.Order.UserId == userId &&
                             oi.Order.OrderStatus == "Completed")
                .OrderBy(oi => oi.Order.CreatedAt)
                .ToListAsync();

            if (!completedOrderItems.Any())
            {
                return (false, "Bạn chỉ có thể đánh giá sản phẩm sau khi đã mua và đơn hàng hoàn tất!", null);
            }

            var userReviews = await _context.Reviews
                .Where(r => r.ProductId == dto.ProductId && r.UserId == userId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            // Nếu số đánh giá đã bằng hoặc vượt quá số lần mua
            if (userReviews.Count >= completedOrderItems.Count)
            {
                return (false, "Bạn đã đánh giá cho lần mua sản phẩm này rồi. Mỗi lần mua hàng hoàn tất chỉ được đánh giá 1 lần!", null);
            }

            // Gắn review với OrderItem của lần mua chưa được đánh giá
            var reviewedOrderItemIds = userReviews
                .Where(r => r.OrderItemId.HasValue)
                .Select(r => r.OrderItemId.Value)
                .ToHashSet();

            var unreviewedOrderItem = completedOrderItems
                .FirstOrDefault(oi => !reviewedOrderItemIds.Contains(oi.OrderItemId))
                ?? completedOrderItems.Last();

            // Tạo đánh giá mới cho lần mua này
            var review = new Review
            {
                ProductId = dto.ProductId,
                UserId = userId,
                OrderItemId = unreviewedOrderItem.OrderItemId,
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
    
        public async Task<ReviewEligibilityDto> CheckEligibilityAsync(string userId, int productId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new ReviewEligibilityDto
                {
                    IsAuthenticated = false,
                    CanReview = false,
                    HasReviewed = false,
                    Message = "Vui lòng đăng nhập để đánh giá sản phẩm."
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ReviewEligibilityDto
                {
                    IsAuthenticated = false,
                    CanReview = false,
                    HasReviewed = false,
                    Message = "Không tìm thấy tài khoản người dùng."
                };
            }

            var completedOrderItems = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.ProductId == productId &&
                             oi.Order.UserId == userId &&
                             oi.Order.OrderStatus == "Completed")
                .ToListAsync();

            var purchaseCount = completedOrderItems.Count;
            if (purchaseCount == 0)
            {
                return new ReviewEligibilityDto
                {
                    IsAuthenticated = true,
                    CanReview = false,
                    HasReviewed = false,
                    Message = "Bạn chỉ có thể đánh giá sản phẩm sau khi mua hàng và đơn hàng đã được giao thành công."
                };
            }

            var userReviews = await _context.Reviews
                .Where(r => r.ProductId == productId && r.UserId == userId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            var reviewCount = userReviews.Count;

            if (reviewCount >= purchaseCount)
            {
                var lastReview = userReviews.LastOrDefault();
                return new ReviewEligibilityDto
                {
                    IsAuthenticated = true,
                    CanReview = false,
                    HasReviewed = true,
                    Message = "Bạn đã đánh giá đủ cho tất cả các lần mua sản phẩm này. Hãy mua thêm để tiếp tục đánh giá!",
                    ExistingReview = lastReview != null ? new ProductReviewItemDto
                    {
                        ReviewId = lastReview.ReviewId,
                        UserId = lastReview.UserId,
                        UserName = user.UserName ?? "Khách hàng",
                        FullName = user.FullName ?? user.UserName ?? "Khách hàng",
                        AvatarUrl = user.AvatarUrl,
                        Rating = lastReview.Rating,
                        Comment = lastReview.Comment,
                        CreatedAt = lastReview.CreatedAt
                    } : null
                };
            }

            var remaining = purchaseCount - reviewCount;
            return new ReviewEligibilityDto
            {
                IsAuthenticated = true,
                CanReview = true,
                HasReviewed = reviewCount > 0,
                Message = reviewCount > 0
                    ? $"Bạn còn {remaining} lượt đánh giá cho các lần mua sản phẩm này."
                    : "Hãy chia sẻ trải nghiệm của bạn về sản phẩm này!"
            };
        }
}
}
