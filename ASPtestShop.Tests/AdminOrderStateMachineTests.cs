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
        [Fact]
        public async Task UpdateOrderStatus_ShouldRejectConfirmation_WhenPaymentStatusIsFailed()
        {
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 501,
                OrderCode = "OD_FAILED_501",
                OrderStatus = "Pending",
                PaymentStatus = "Failed",
                OrderItems = new List<OrderItem>()
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var service = new AdminOrderService(context);
            var result = await service.UpdateOrderStatusAsync(501, new UpdateOrderStatusDto
            {
                OrderStatus = "Confirmed",
                PaymentStatus = "Failed"
            });

            Assert.False(result.Success);
            Assert.Contains("thất bại", result.Message);
        }

        [Fact]
        public async Task BatchConfirmOrders_ShouldSkipOrders_WhenPaymentStatusIsFailed()
        {
            using var context = CreateInMemoryDbContext();
            context.Orders.AddRange(
                new Order { OrderId = 502, OrderCode = "OD_502_PAID", OrderStatus = "Pending", PaymentStatus = "Paid" },
                new Order { OrderId = 503, OrderCode = "OD_503_FAILED", OrderStatus = "Pending", PaymentStatus = "Failed" }
            );
            await context.SaveChangesAsync();

            var service = new AdminOrderService(context);
            var result = await service.BatchConfirmOrdersAsync(new List<int> { 502, 503 });

            Assert.True(result.Success);
            Assert.Equal(1, result.SuccessCount);
            Assert.Equal(1, result.FailedCount);

            var o502 = await context.Orders.FindAsync(502);
            var o503 = await context.Orders.FindAsync(503);
            Assert.Equal("Confirmed", o502?.OrderStatus);
            Assert.Equal("Pending", o503?.OrderStatus); // Vẫn giữ Pending, không bị duyệt
        }

        [Fact]
        public async Task BatchDeleteOrders_ShouldDeleteCancelledAndPendingOrders_AndRestockIfPending()
        {
            using var context = CreateInMemoryDbContext();

            var product = new Product
            {
                ProductId = 101,
                ProductName = "Cần câu cao cấp",
                Slug = "can-cau-cao-cap",
                Price = 500000,
                StockQuantity = 10
            };
            context.Products.Add(product);

            var pendingOrder = new Order
            {
                OrderId = 601,
                OrderCode = "OD_601_PENDING",
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { OrderItemId = 1, ProductId = 101, Quantity = 3, UnitPrice = 500000, LineTotal = 1500000 }
                }
            };

            var cancelledOrder = new Order
            {
                OrderId = 602,
                OrderCode = "OD_602_CANCELLED",
                OrderStatus = "Cancelled",
                PaymentStatus = "Unpaid",
                OrderItems = new List<OrderItem>()
            };

            var completedOrder = new Order
            {
                OrderId = 603,
                OrderCode = "OD_603_COMPLETED",
                OrderStatus = "Completed",
                PaymentStatus = "Paid",
                OrderItems = new List<OrderItem>()
            };

            context.Orders.AddRange(pendingOrder, cancelledOrder, completedOrder);
            await context.SaveChangesAsync();

            var service = new AdminOrderService(context);
            var result = await service.BatchDeleteOrdersAsync(new List<int> { 601, 602, 603 });

            Assert.True(result.Success);
            Assert.Equal(3, result.SuccessCount); // Xóa tự do cả 3 đơn được chọn
            Assert.Equal(0, result.FailedCount);

            var db601 = await context.Orders.FindAsync(601);
            var db602 = await context.Orders.FindAsync(602);
            var db603 = await context.Orders.FindAsync(603);
            Assert.Null(db601);
            Assert.Null(db602);
            Assert.Null(db603);

            // Tồn kho sản phẩm của đơn Pending được hoàn lại: 10 + 3 = 13
            var updatedProduct = await context.Products.FindAsync(101);
            Assert.Equal(13, updatedProduct?.StockQuantity);
        }
    }
}
