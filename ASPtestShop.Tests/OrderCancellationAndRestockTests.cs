using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Models.DTO.Refund;
using ASPtestShop.Services.Implementations;
using ASPtestShop.Services.Implementations.Admin;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASPtestShop.Tests
{
    public class OrderCancellationAndRestockTests
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
        public async Task CancelOrderAsync_WhenPendingAndUnpaid_RestocksInventoryAndCancelsOrder()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductId = 101,
                ProductName = "Cần câu Fuu Carbon",
                Slug = "can-cau-fuu-carbon",
                StockQuantity = 10,
                Price = 500000
            };
            context.Products.Add(product);

            var order = new Order
            {
                OrderId = 201,
                OrderCode = "OD_TEST_201",
                UserId = "user-1",
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                FinalAmount = 500000,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { OrderItemId = 1, ProductId = 101, Quantity = 3, UnitPrice = 500000, LineTotal = 1500000 }
                }
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var orderService = new OrderService(context, null!, null!);

            // Act
            var dto = new CancelOrderRequestDto { Reason = "Đổi ý không mua nữa" };
            var result = await orderService.CancelOrderAsync("user-1", 201, dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Cancelled", result.OrderStatus);

            var updatedOrder = await context.Orders.FindAsync(201);
            Assert.NotNull(updatedOrder);
            Assert.Equal("Cancelled", updatedOrder.OrderStatus);
            Assert.Contains("Đổi ý không mua nữa", updatedOrder.Note);

            var updatedProduct = await context.Products.FindAsync(101);
            Assert.NotNull(updatedProduct);
            Assert.Equal(13, updatedProduct.StockQuantity); // 10 + 3 = 13
        }

        [Fact]
        public async Task CancelOrderAsync_WhenPendingAndPaid_SetsRefundPendingAndCreatesRefundRequest()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductId = 102,
                ProductName = "Máy câu Daiwa",
                Slug = "may-cau-daiwa",
                StockQuantity = 5,
                Price = 1200000
            };
            context.Products.Add(product);

            var order = new Order
            {
                OrderId = 202,
                OrderCode = "OD_TEST_202",
                UserId = "user-paid-1",
                OrderStatus = "Pending",
                PaymentStatus = "Paid",
                PaymentMethod = "VNPAY",
                FinalAmount = 1200000,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { OrderItemId = 2, ProductId = 102, Quantity = 2, UnitPrice = 1200000, LineTotal = 2400000 }
                }
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var orderService = new OrderService(context, null!, null!);

            // Act
            var dto = new CancelOrderRequestDto
            {
                Reason = "Hủy đơn online đã thanh toán",
                BankName = "Vietcombank",
                AccountNumber = "0123456789",
                AccountHolderName = "NGUYEN VAN A"
            };
            var result = await orderService.CancelOrderAsync("user-paid-1", 202, dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Cancelled", result.OrderStatus);

            var updatedOrder = await context.Orders.FindAsync(202);
            Assert.NotNull(updatedOrder);
            Assert.Equal("Cancelled", updatedOrder.OrderStatus);
            Assert.Equal("RefundPending", updatedOrder.PaymentStatus);

            // Kiểm tra RefundRequest đã được tạo tự động
            var refund = await context.RefundRequests.FirstOrDefaultAsync(r => r.OrderId == 202);
            Assert.NotNull(refund);
            Assert.Equal("Pending", refund.Status);
            Assert.Equal(1200000, refund.Amount);
            Assert.Equal("Vietcombank", refund.BankName);
            Assert.Equal("0123456789", refund.AccountNumber);

            // Kiểm tra sản phẩm đã được hoàn kho ngay
            var updatedProduct = await context.Products.FindAsync(102);
            Assert.NotNull(updatedProduct);
            Assert.Equal(7, updatedProduct.StockQuantity); // 5 + 2 = 7
        }

        [Fact]
        public async Task RequestCancelOrderAsync_WhenConfirmed_MarksCancellationRequest_DoesNotRestockYet()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductId = 103,
                ProductName = "Dây câu PE X8",
                Slug = "day-cau-pe-x8",
                StockQuantity = 20,
                Price = 150000
            };
            context.Products.Add(product);

            var order = new Order
            {
                OrderId = 203,
                OrderCode = "OD_TEST_203",
                UserId = "user-confirmed-1",
                OrderStatus = "Confirmed",
                PaymentStatus = "Unpaid",
                FinalAmount = 300000,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { OrderItemId = 3, ProductId = 103, Quantity = 2, UnitPrice = 150000, LineTotal = 300000 }
                }
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var orderService = new OrderService(context, null!, null!);

            // Act
            var result = await orderService.RequestCancelOrderAsync("user-confirmed-1", 203, "Cần đổi lại địa chỉ nhận hàng");

            // Assert
            Assert.True(result.Success);
            var updatedOrder = await context.Orders.FindAsync(203);
            Assert.NotNull(updatedOrder);
            Assert.Equal("Confirmed", updatedOrder.OrderStatus); // Vẫn Confirmed chờ duyệt
            Assert.Contains("[Yêu cầu hủy]", updatedOrder.Note);

            // Tồn kho chưa được hoàn khi chưa có duyệt từ Admin
            var productInDb = await context.Products.FindAsync(103);
            Assert.NotNull(productInDb);
            Assert.Equal(20, productInDb.StockQuantity);
        }

        [Fact]
        public async Task AdminApproveCancelOrder_WhenOrderHasCancelRequest_CancelsOrderAndRestocks()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductId = 104,
                ProductName = "Mồi câu cá giả",
                Slug = "moi-cau-ca-gia",
                StockQuantity = 15,
                Price = 80000
            };
            context.Products.Add(product);

            var order = new Order
            {
                OrderId = 204,
                OrderCode = "OD_TEST_204",
                UserId = "user-4",
                OrderStatus = "Confirmed",
                PaymentStatus = "Unpaid",
                Note = "[Yêu cầu hủy]: Muốn đổi sang mẫu khác",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { OrderItemId = 4, ProductId = 104, Quantity = 5, UnitPrice = 80000, LineTotal = 400000 }
                }
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var adminOrderService = new AdminOrderService(context);

            // Act: Admin duyệt yêu cầu hủy
            var result = await adminOrderService.ApproveCancelOrderAsync(204, "Đồng ý hủy đơn theo yêu cầu");

            // Assert
            Assert.True(result.Success);
            var updatedOrder = await context.Orders.FindAsync(204);
            Assert.NotNull(updatedOrder);
            Assert.Equal("Cancelled", updatedOrder.OrderStatus);

            // Tồn kho được hoàn lại đầy đủ: 15 + 5 = 20
            var updatedProduct = await context.Products.FindAsync(104);
            Assert.NotNull(updatedProduct);
            Assert.Equal(20, updatedProduct.StockQuantity);
        }

        [Fact]
        public async Task ProcessRefund_WhenOrderAlreadyCancelled_DoesNotRestockTwice()
        {
            // Arrange: Đơn hàng đã Cancelled và đã hoàn kho trước đó
            using var context = CreateInMemoryDbContext();
            var product = new Product
            {
                ProductId = 105,
                ProductName = "Phao câu đêm",
                Slug = "phao-cau-dem",
                StockQuantity = 10,
                Price = 50000
            };
            context.Products.Add(product);

            var order = new Order
            {
                OrderId = 205,
                OrderCode = "OD_TEST_205",
                UserId = "user-5",
                OrderStatus = "Cancelled", // Đã hủy
                PaymentStatus = "RefundPending", // Đang chờ hoàn tiền
                FinalAmount = 100000,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { OrderItemId = 5, ProductId = 105, Quantity = 2, UnitPrice = 50000, LineTotal = 100000 }
                }
            };
            context.Orders.Add(order);

            var refundRequest = new RefundRequest
            {
                RefundRequestId = 305,
                OrderId = 205,
                UserId = "user-5",
                Amount = 100000,
                Reason = "Khách hủy đơn online",
                Status = "Pending"
            };
            context.RefundRequests.Add(refundRequest);
            await context.SaveChangesAsync();

            var refundService = new RefundService(context);

            // Act: Admin duyệt hoàn tiền
            var result = await refundService.ProcessRefundAsync(305, "admin-1", new ProcessRefundDto
            {
                Status = "Approved",
                AdminNote = "Đã chuyển khoản trả lại tiền"
            });

            // Assert
            Assert.True(result.Success);
            var updatedOrder = await context.Orders.FindAsync(205);
            Assert.NotNull(updatedOrder);
            Assert.Equal("Refunded", updatedOrder.PaymentStatus);

            // Số lượng kho phải giữ nguyên 10, KHÔNG ĐƯỢC cộng thêm 2 lần nữa thành 12!
            var updatedProduct = await context.Products.FindAsync(105);
            Assert.NotNull(updatedProduct);
            Assert.Equal(10, updatedProduct.StockQuantity);
        }
    }
}
