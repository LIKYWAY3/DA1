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
    }
}
