using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Services.Implementations.Admin;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASPtestShop.Tests
{
    public class AdminOrderStateMachineTests
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
        public async Task UpdateOrderStatus_ShouldRejectTransition_WhenOrderIsAlreadyCancelled()
        {
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 301,
                OrderCode = "OD_CANCELLED_301",
                OrderStatus = "Cancelled",
                PaymentStatus = "Unpaid",
                OrderItems = new List<OrderItem>()
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var service = new AdminOrderService(context);
            var result = await service.UpdateOrderStatusAsync(301, new UpdateOrderStatusDto
            {
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid"
            });

            Assert.False(result.Success);
            Assert.Contains("Đơn hàng đã hủy", result.Message);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldRejectTransition_WhenOrderIsAlreadyCompleted()
        {
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 302,
                OrderCode = "OD_COMPLETED_302",
                OrderStatus = "Completed",
                PaymentStatus = "Paid",
                OrderItems = new List<OrderItem>()
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var service = new AdminOrderService(context);
            var result = await service.UpdateOrderStatusAsync(302, new UpdateOrderStatusDto
            {
                OrderStatus = "Confirmed",
                PaymentStatus = "Paid"
            });

            Assert.False(result.Success);
            Assert.Contains("Đơn hàng đã hoàn tất", result.Message);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldAllowValidTransition_WhenPendingToConfirmed()
        {
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 303,
                OrderCode = "OD_PENDING_303",
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                OrderItems = new List<OrderItem>()
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var service = new AdminOrderService(context);
            var result = await service.UpdateOrderStatusAsync(303, new UpdateOrderStatusDto
            {
                OrderStatus = "Confirmed",
                PaymentStatus = "Unpaid"
            });

            Assert.True(result.Success);
            var updated = await context.Orders.FindAsync(303);
            Assert.Equal("Confirmed", updated?.OrderStatus);
        }

        [Fact]
        public async Task BatchConfirmOrders_ShouldConfirmAllPendingOrders_WhenValidIdsProvided()
        {
            using var context = CreateInMemoryDbContext();
            context.Orders.AddRange(
                new Order { OrderId = 401, OrderCode = "OD_401", OrderStatus = "Pending", PaymentStatus = "Unpaid" },
                new Order { OrderId = 402, OrderCode = "OD_402", OrderStatus = "Pending", PaymentStatus = "Unpaid" }
            );
            await context.SaveChangesAsync();

            var service = new AdminOrderService(context);
            var result = await service.BatchConfirmOrdersAsync(new List<int> { 401, 402 });

            Assert.True(result.Success);
            Assert.Equal(2, result.SuccessCount);
            Assert.Equal(0, result.FailedCount);

            var o1 = await context.Orders.FindAsync(401);
            var o2 = await context.Orders.FindAsync(402);
            Assert.Equal("Confirmed", o1?.OrderStatus);
            Assert.Equal("Confirmed", o2?.OrderStatus);
            Assert.Contains("[Admin duyệt hàng loạt]", o1?.Note);
        }

        [Fact]
        public async Task BatchConfirmOrders_ShouldOnlyConfirmPendingOrders_AndSkipNonPending()
        {
            using var context = CreateInMemoryDbContext();
            context.Orders.AddRange(
                new Order { OrderId = 403, OrderCode = "OD_403", OrderStatus = "Pending", PaymentStatus = "Unpaid" },
                new Order { OrderId = 404, OrderCode = "OD_404", OrderStatus = "Cancelled", PaymentStatus = "Unpaid" }
            );
            await context.SaveChangesAsync();

            var service = new AdminOrderService(context);
            var result = await service.BatchConfirmOrdersAsync(new List<int> { 403, 404 });

            Assert.True(result.Success);
            Assert.Equal(1, result.SuccessCount);
            Assert.Equal(1, result.FailedCount);

            var o3 = await context.Orders.FindAsync(403);
            var o4 = await context.Orders.FindAsync(404);
            Assert.Equal("Confirmed", o3?.OrderStatus);
            Assert.Equal("Cancelled", o4?.OrderStatus);
        }

        [Fact]
        public async Task BatchConfirmOrders_ShouldReturnError_WhenNoPendingOrdersExist()
        {
            using var context = CreateInMemoryDbContext();
            context.Orders.Add(new Order { OrderId = 405, OrderCode = "OD_405", OrderStatus = "Completed", PaymentStatus = "Paid" });
            await context.SaveChangesAsync();

            var service = new AdminOrderService(context);
            var result = await service.BatchConfirmOrdersAsync(new List<int> { 405 });

            Assert.False(result.Success);
            Assert.Contains("Không có đơn hàng nào", result.Message);
        }
    }
}
