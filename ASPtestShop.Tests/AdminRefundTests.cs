using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Refund;
using ASPtestShop.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASPtestShop.Tests
{
    public class AdminRefundTests
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
        public async Task ProcessRefundAsync_WhenApprovedWithTransactionCode_SucceedsAndRecordsCode()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 401,
                OrderCode = "OD_REFUND_401",
                UserId = "user-rf-1",
                OrderStatus = "Completed",
                PaymentStatus = "RefundPending",
                FinalAmount = 250000
            };
            context.Orders.Add(order);

            var refund = new RefundRequest
            {
                RefundRequestId = 501,
                OrderId = 401,
                UserId = "user-rf-1",
                Amount = 250000,
                Reason = "Sản phẩm lỗi khi nhận",
                Status = "Pending",
                BankName = "Vietcombank",
                AccountNumber = "0987654321",
                AccountHolderName = "NGUYEN VAN A"
            };
            context.RefundRequests.Add(refund);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            // Act
            var dto = new ProcessRefundDto
            {
                Status = "Approved",
                TransactionCode = "VCB_TX_999888",
                AdminNote = "Đã hoàn qua ngân hàng số VCB"
            };
            var result = await service.ProcessRefundAsync(501, "admin-1", dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Approved", result.Status);

            var updatedRefund = await context.RefundRequests.FindAsync(501);
            Assert.NotNull(updatedRefund);
            Assert.Equal("Approved", updatedRefund.Status);
            Assert.Contains("[Mã GD: VCB_TX_999888]", updatedRefund.AdminNote);

            var updatedOrder = await context.Orders.FindAsync(401);
            Assert.NotNull(updatedOrder);
            Assert.Equal("Refunded", updatedOrder.PaymentStatus);
        }

        [Fact]
        public async Task ProcessRefundAsync_WhenRejectedWithoutAdminNote_ReturnsFailure()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 402,
                OrderCode = "OD_REFUND_402",
                UserId = "user-rf-2",
                OrderStatus = "Completed",
                PaymentStatus = "RefundPending",
                FinalAmount = 150000
            };
            context.Orders.Add(order);

            var refund = new RefundRequest
            {
                RefundRequestId = 502,
                OrderId = 402,
                UserId = "user-rf-2",
                Amount = 150000,
                Reason = "Khách yêu cầu",
                Status = "Pending"
            };
            context.RefundRequests.Add(refund);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            // Act: Reject without AdminNote
            var dto = new ProcessRefundDto
            {
                Status = "Rejected",
                AdminNote = "" // Không nhập lý do
            };
            var result = await service.ProcessRefundAsync(502, "admin-1", dto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("lý do từ chối", result.Message);

            var unchangedRefund = await context.RefundRequests.FindAsync(502);
            Assert.NotNull(unchangedRefund);
            Assert.Equal("Pending", unchangedRefund.Status); // Vẫn Pending
        }

        [Fact]
        public async Task ProcessRefundAsync_WhenRejectedWithAdminNote_SetsStatusRejectedAndRevertsOrderPaymentStatus()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 403,
                OrderCode = "OD_REFUND_403",
                UserId = "user-rf-3",
                OrderStatus = "Completed",
                PaymentStatus = "RefundPending",
                FinalAmount = 300000
            };
            context.Orders.Add(order);

            var refund = new RefundRequest
            {
                RefundRequestId = 503,
                OrderId = 403,
                UserId = "user-rf-3",
                Amount = 300000,
                Reason = "Không ưng ý",
                Status = "Pending"
            };
            context.RefundRequests.Add(refund);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            // Act
            var dto = new ProcessRefundDto
            {
                Status = "Rejected",
                AdminNote = "Sản phẩm đã qua sử dụng quá 7 ngày, không đủ điều kiện đổi trả"
            };
            var result = await service.ProcessRefundAsync(503, "admin-1", dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Rejected", result.Status);

            var updatedRefund = await context.RefundRequests.FindAsync(503);
            Assert.NotNull(updatedRefund);
            Assert.Equal("Rejected", updatedRefund.Status);
            Assert.Equal("Sản phẩm đã qua sử dụng quá 7 ngày, không đủ điều kiện đổi trả", updatedRefund.AdminNote);

            var updatedOrder = await context.Orders.FindAsync(403);
            Assert.NotNull(updatedOrder);
            Assert.Equal("Paid", updatedOrder.PaymentStatus);
        }

        [Fact]
        public async Task GetPendingRefundCountAsync_CountsOnlyPendingRequests()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            context.RefundRequests.AddRange(
                new RefundRequest { RefundRequestId = 601, OrderId = 1, UserId = "u1", Amount = 100, Reason = "R1", Status = "Pending" },
                new RefundRequest { RefundRequestId = 602, OrderId = 2, UserId = "u2", Amount = 200, Reason = "R2", Status = "Pending" },
                new RefundRequest { RefundRequestId = 603, OrderId = 3, UserId = "u3", Amount = 300, Reason = "R3", Status = "Approved" },
                new RefundRequest { RefundRequestId = 604, OrderId = 4, UserId = "u4", Amount = 400, Reason = "R4", Status = "Rejected" }
            );
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            // Act
            var count = await service.GetPendingRefundCountAsync();

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task ResubmitRefundRequestAsync_WhenRejected_UpdatesBankInfoAndResetsStatusToPending()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 405,
                OrderCode = "OD_REFUND_405",
                UserId = "user-rf-5",
                OrderStatus = "Cancelled",
                PaymentStatus = "Paid",
                FinalAmount = 500000
            };
            context.Orders.Add(order);

            var refund = new RefundRequest
            {
                RefundRequestId = 505,
                OrderId = 405,
                UserId = "user-rf-5",
                Amount = 500000,
                Reason = "Hủy đơn hàng",
                Status = "Rejected",
                BankName = "Sai Bank",
                AccountNumber = "000000000",
                AccountHolderName = "Sai Ten",
                AdminNote = "Sai số tài khoản, vui lòng cập nhật lại"
            };
            context.RefundRequests.Add(refund);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            // Act: Khách gửi lại thông tin chuẩn
            var dto = new ResubmitRefundRequestDto
            {
                OrderId = 405,
                BankName = "Techcombank",
                AccountNumber = "190333444555",
                AccountHolderName = "NGUYEN VAN DUNG",
                Reason = "Đã sửa lại đúng STK Techcombank"
            };
            var result = await service.ResubmitRefundRequestAsync("user-rf-5", dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Pending", result.Status);

            var updatedRefund = await context.RefundRequests.FindAsync(505);
            Assert.NotNull(updatedRefund);
            Assert.Equal("Pending", updatedRefund.Status);
            Assert.Equal("Techcombank", updatedRefund.BankName);
            Assert.Equal("190333444555", updatedRefund.AccountNumber);
            Assert.Equal("NGUYEN VAN DUNG", updatedRefund.AccountHolderName);
            Assert.Null(updatedRefund.AdminNote); // Đã xóa note cũ để duyệt mới

            var updatedOrder = await context.Orders.FindAsync(405);
            Assert.NotNull(updatedOrder);
            Assert.Equal("RefundPending", updatedOrder.PaymentStatus);
        }

        [Fact]
        public async Task ResubmitRefundRequestAsync_WhenStatusIsNotRejected_ReturnsFailure()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 406,
                OrderCode = "OD_REFUND_406",
                UserId = "user-rf-6",
                OrderStatus = "Completed",
                PaymentStatus = "RefundPending",
                FinalAmount = 100000
            };
            context.Orders.Add(order);

            var refund = new RefundRequest
            {
                RefundRequestId = 506,
                OrderId = 406,
                UserId = "user-rf-6",
                Amount = 100000,
                Reason = "Lỗi sản phẩm",
                Status = "Pending" // Đang Pending, không phải Rejected
            };
            context.RefundRequests.Add(refund);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            // Act
            var dto = new ResubmitRefundRequestDto
            {
                OrderId = 406,
                BankName = "MBBank",
                AccountNumber = "111222333",
                AccountHolderName = "TRAN VAN B"
            };
            var result = await service.ResubmitRefundRequestAsync("user-rf-6", dto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("trạng thái 'Rejected'", result.Message);
        }
    }
}
