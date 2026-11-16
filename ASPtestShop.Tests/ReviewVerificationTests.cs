using System;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Review;
using ASPtestShop.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class ReviewVerificationTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new AppDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager(ApplicationUser user)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            mgr.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
            return mgr;
        }

        [Fact]
        public async Task CreateReview_ShouldFail_WhenUserHasNotPurchasedProduct()
        {
            using var context = CreateInMemoryDbContext();
            var user = new ApplicationUser { Id = "buyer1", UserName = "buyer1" };
            var product = new Product { ProductId = 201, ProductName = "Fishing Net", Slug = "fishing-net", Price = 100000 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var userManager = MockUserManager(user);
            var reviewService = new ReviewService(context, userManager.Object);

            var (success, message, _) = await reviewService.CreateReviewAsync("buyer1", new CreateReviewDto
            {
                ProductId = 201,
                Rating = 5,
                Comment = "Great net!"
            });

            Assert.False(success);
            Assert.Contains("Bạn chỉ có thể đánh giá sản phẩm sau khi đã mua", message);
        }

        [Fact]
        public async Task CreateReview_ShouldSucceed_AndSetOrderItemId_WhenOrderIsCompleted()
        {
            using var context = CreateInMemoryDbContext();
            var user = new ApplicationUser { Id = "buyer2", UserName = "buyer2" };
            var product = new Product { ProductId = 202, ProductName = "Lure Pack", Slug = "lure-pack", Price = 150000 };
            context.Products.Add(product);

            var order = new Order
            {
                OrderId = 501,
                OrderCode = "OD_COMPLETED_501",
                UserId = "buyer2",
                OrderStatus = "Completed",
                CreatedAt = DateTime.UtcNow
            };
            context.Orders.Add(order);

            var orderItem = new OrderItem
            {
                OrderItemId = 1001,
                OrderId = 501,
                ProductId = 202,
                Quantity = 1,
                UnitPrice = 150000
            };
            context.OrderItems.Add(orderItem);
            await context.SaveChangesAsync();

            var userManager = MockUserManager(user);
            var reviewService = new ReviewService(context, userManager.Object);

            var (success, _, resultDto) = await reviewService.CreateReviewAsync("buyer2", new CreateReviewDto
            {
                ProductId = 202,
                Rating = 5,
                Comment = "High quality lures!"
            });

            Assert.True(success);
            Assert.NotNull(resultDto);

            var savedReview = await context.Reviews.FirstOrDefaultAsync(r => r.ProductId == 202 && r.UserId == "buyer2");
            Assert.NotNull(savedReview);
            Assert.Equal(1001, savedReview.OrderItemId);
        }

        [Fact]
        public async Task CreateReview_ShouldBlockSecondReview_WhenUserOnlyPurchasedOnce()
        {
            using var context = CreateInMemoryDbContext();
            var user = new ApplicationUser { Id = "single_buyer", UserName = "single_buyer" };
            var product = new Product { ProductId = 203, ProductName = "Fishing Rod", Slug = "fishing-rod", Price = 500000 };
            context.Products.Add(product);

            var order = new Order { OrderId = 601, OrderCode = "OD_601", UserId = "single_buyer", OrderStatus = "Completed" };
            context.Orders.Add(order);
            context.OrderItems.Add(new OrderItem { OrderItemId = 2001, OrderId = 601, ProductId = 203, Quantity = 1, UnitPrice = 500000 });
            await context.SaveChangesAsync();

            var userManager = MockUserManager(user);
            var reviewService = new ReviewService(context, userManager.Object);

            // Review 1: Should succeed
            var (s1, _, _) = await reviewService.CreateReviewAsync("single_buyer", new CreateReviewDto
            {
                ProductId = 203,
                Rating = 5,
                Comment = "Cần câu rất tốt!"
            });
            Assert.True(s1);

            // Review 2: Should be blocked because only 1 purchase
            var (s2, m2, _) = await reviewService.CreateReviewAsync("single_buyer", new CreateReviewDto
            {
                ProductId = 203,
                Rating = 4,
                Comment = "Đánh giá lần 2 bị chặn"
            });
            Assert.False(s2);
            Assert.Contains("Mỗi lần mua hàng hoàn tất chỉ được đánh giá 1 lần", m2);

            var elig = await reviewService.CheckEligibilityAsync("single_buyer", 203);
            Assert.False(elig.CanReview);
            Assert.True(elig.HasReviewed);
            Assert.Contains("Bạn đã đánh giá đủ cho tất cả các lần mua", elig.Message);
        }

        [Fact]
        public async Task CreateReview_ShouldAllowMultipleReviews_WhenUserPurchasedMultipleTimes()
        {
            using var context = CreateInMemoryDbContext();
            var user = new ApplicationUser { Id = "repeat_buyer", UserName = "repeat_buyer" };
            var product = new Product { ProductId = 204, ProductName = "Fishing Reel", Slug = "fishing-reel", Price = 300000 };
            context.Products.Add(product);

            // Purchase 1
            var o1 = new Order { OrderId = 701, OrderCode = "OD_701", UserId = "repeat_buyer", OrderStatus = "Completed" };
            context.Orders.Add(o1);
            context.OrderItems.Add(new OrderItem { OrderItemId = 3001, OrderId = 701, ProductId = 204, Quantity = 1, UnitPrice = 300000 });

            // Purchase 2
            var o2 = new Order { OrderId = 702, OrderCode = "OD_702", UserId = "repeat_buyer", OrderStatus = "Completed" };
            context.Orders.Add(o2);
            context.OrderItems.Add(new OrderItem { OrderItemId = 3002, OrderId = 702, ProductId = 204, Quantity = 1, UnitPrice = 300000 });
            await context.SaveChangesAsync();

            var userManager = MockUserManager(user);
            var reviewService = new ReviewService(context, userManager.Object);

            // Review 1
            var (s1, _, _) = await reviewService.CreateReviewAsync("repeat_buyer", new CreateReviewDto
            {
                ProductId = 204,
                Rating = 5,
                Comment = "Lần 1 mua dùng tốt"
            });
            Assert.True(s1);

            // Eligibility check: should still have 1 remaining review
            var elig = await reviewService.CheckEligibilityAsync("repeat_buyer", 204);
            Assert.True(elig.CanReview);
            Assert.Contains("Bạn còn 1 lượt đánh giá", elig.Message);

            // Review 2
            var (s2, _, _) = await reviewService.CreateReviewAsync("repeat_buyer", new CreateReviewDto
            {
                ProductId = 204,
                Rating = 5,
                Comment = "Lần 2 mua thêm tặng bạn bè"
            });
            Assert.True(s2);

            // Review 3: Should now be blocked
            var (s3, m3, _) = await reviewService.CreateReviewAsync("repeat_buyer", new CreateReviewDto
            {
                ProductId = 204,
                Rating = 3,
                Comment = "Lần 3 chưa mua thêm"
            });
            Assert.False(s3);
            Assert.Contains("Mỗi lần mua hàng hoàn tất chỉ được đánh giá 1 lần", m3);
        }
    }
}
