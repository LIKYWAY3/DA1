using System;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASPtestShop.Tests
{
    public class OrderTimeoutTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AutoCancelExpiredOrders_ShouldNotCancelFreshOnlineOrder_WithinTimeout()
        {
            // Arrange: Đơn hàng mới tạo cách đây 5 phút (hạn chót là 30 phút)
            using var context = CreateInMemoryDbContext();

            var freshOnlineOrder = new Order
            {
                OrderCode = "OD_FRESH_ONLINE",
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                PaymentMethod = "VNPAY",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5) // Mới tạo 5 phút trước theo UTC
            };

            context.Orders.Add(freshOnlineOrder);
            await context.SaveChangesAsync();

            var orderService = new OrderService(context, null!, null!);

            // Act: Chạy quét hủy đơn quá hạn
            var result = await orderService.AutoCancelExpiredOrdersAsync(timeoutHours: 24, onlineTimeoutMinutes: 30);

            // Assert: Đơn hàng mới tạo 5 phút không được phép bị hủy
            var checkedOrder = await context.Orders.FirstOrDefaultAsync(o => o.OrderCode == "OD_FRESH_ONLINE");
            Assert.NotNull(checkedOrder);
            Assert.Equal("Pending", checkedOrder.OrderStatus);
            Assert.Equal("Unpaid", checkedOrder.PaymentStatus);
        }

        [Fact]
        public async Task AutoCancelExpiredOrders_ShouldCancelTrulyExpiredOnlineOrder()
        {
            // Arrange: Đơn hàng tạo 40 phút trước theo UTC (vượt quá 30 phút online timeout)
            using var context = CreateInMemoryDbContext();

            var expiredOrder = new Order
            {
                OrderCode = "OD_EXPIRED_ONLINE",
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                PaymentMethod = "MOMO",
                CreatedAt = DateTime.UtcNow.AddMinutes(-40)
            };

            context.Orders.Add(expiredOrder);
            await context.SaveChangesAsync();

            var orderService = new OrderService(context, null!, null!);

            // Act
            await orderService.AutoCancelExpiredOrdersAsync(timeoutHours: 24, onlineTimeoutMinutes: 30);

            // Assert
            var checkedOrder = await context.Orders.FirstOrDefaultAsync(o => o.OrderCode == "OD_EXPIRED_ONLINE");
            Assert.NotNull(checkedOrder);
            Assert.Equal("Cancelled", checkedOrder.OrderStatus);
        }
    }
}
