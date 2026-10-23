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
    }
}
