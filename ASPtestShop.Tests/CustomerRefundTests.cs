using System;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Refund;
using ASPtestShop.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASPtestShop.Tests
{
    /// <summary>
    /// UC-20: Kiểm thử nghiệp vụ Khách hàng yêu cầu hoàn tiền (Customer Refund & Anti-IDOR Tests)
    /// </summary>
    public class CustomerRefundTests
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
        public async Task RequestRefundAsync_WhenOwnerRequestsRefundForPaidOrder_SucceedsAndSetsRefundPending()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 701,
                OrderCode = "OD_CUST_701",
                UserId = "customer-123",
                OrderStatus = "Completed",
                PaymentStatus = "Paid",
                FinalAmount = 1500000
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            var dto = new CreateRefundRequestDto
            {
                OrderId = 701,
                Reason = "Sản phẩm bị trầy xước nặng khi giao",
                BankName = "Vietcombank",
                AccountNumber = "9943265155",
                AccountHolderName = "NGUYEN HUYNH KHA"
            };

            // Act: Chính chủ gửi yêu cầu
            var result = await service.RequestRefundAsync("customer-123", dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Pending", result.Status);

            var updatedOrder = await context.Orders.FindAsync(701);
            Assert.NotNull(updatedOrder);
            Assert.Equal("RefundPending", updatedOrder.PaymentStatus);

            var refund = await context.RefundRequests.FirstOrDefaultAsync(r => r.OrderId == 701);
            Assert.NotNull(refund);
            Assert.Equal("customer-123", refund.UserId);
            Assert.Equal(1500000, refund.Amount);
            Assert.Equal("Vietcombank", refund.BankName);
            Assert.Equal("9943265155", refund.AccountNumber);
            Assert.Equal("NGUYEN HUYNH KHA", refund.AccountHolderName);
        }

        [Fact]
        public async Task RequestRefundAsync_WhenDifferentUserTriesToRequestRefund_FailsWithAntiIDOR()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 702,
                OrderCode = "OD_CUST_702",
                UserId = "customer-legit",
                OrderStatus = "Completed",
                PaymentStatus = "Paid",
                FinalAmount = 2500000
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            var dto = new CreateRefundRequestDto
            {
                OrderId = 702,
                Reason = "Kẻ gian cố tình đòi hoàn tiền của người khác",
                BankName = "MBBank",
                AccountNumber = "000111222",
                AccountHolderName = "ATTACKER"
            };

            // Act: Kẻ gian (customer-attacker) gọi hoàn tiền đơn của customer-legit
            var result = await service.RequestRefundAsync("customer-attacker", dto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("không thuộc về bạn", result.Message);

            var orderCheck = await context.Orders.FindAsync(702);
            Assert.NotNull(orderCheck);
            Assert.Equal("Paid", orderCheck.PaymentStatus); // Không bị đổi trạng thái
        }

        [Fact]
        public async Task RequestRefundAsync_WhenAdminRequestsRefundForCustomerOrder_Succeeds()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 703,
                OrderCode = "OD_CUST_703",
                UserId = "customer-regular",
                OrderStatus = "Completed",
                PaymentStatus = "Paid",
                FinalAmount = 3500000
            };
            context.Orders.Add(order);

            // Tạo Role Admin và gán cho admin-user
            var adminRole = new IdentityRole { Id = "role-admin", Name = "Admin", NormalizedName = "ADMIN" };
            context.Roles.Add(adminRole);
            context.UserRoles.Add(new IdentityUserRole<string> { UserId = "admin-user", RoleId = "role-admin" });
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            var dto = new CreateRefundRequestDto
            {
                OrderId = 703,
                Reason = "Admin hỗ trợ hoàn tiền theo khiếu nại của khách",
                BankName = "Techcombank",
                AccountNumber = "190333888999",
                AccountHolderName = "CUSTOMER REGULAR"
            };

            // Act: Admin gọi hỗ trợ
            var result = await service.RequestRefundAsync("admin-user", dto);

            // Assert
            Assert.True(result.Success);
            var refund = await context.RefundRequests.FirstOrDefaultAsync(r => r.OrderId == 703);
            Assert.NotNull(refund);
            Assert.Equal("customer-regular", refund.UserId); // Luôn gắn với chủ đơn hàng
            Assert.Equal(3500000, refund.Amount);
        }

        [Fact]
        public async Task RequestRefundAsync_WhenBankDetailsOmitted_AutofillsDefaultBankAccount()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 704,
                OrderCode = "OD_CUST_704",
                UserId = "customer-with-bank",
                OrderStatus = "Completed",
                PaymentStatus = "Paid",
                FinalAmount = 890000
            };
            context.Orders.Add(order);

            // Tạo tài khoản ngân hàng mặc định trong BankAccounts (tích hợp UC-05 -> UC-20)
            var defaultBank = new BankAccount
            {
                Id = 91,
                UserId = "customer-with-bank",
                BankName = "Vietcombank",
                AccountNumber = "9943265155",
                AccountName = "NGUYEN HUYNH KHA",
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            };
            context.BankAccounts.Add(defaultBank);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            var dto = new CreateRefundRequestDto
            {
                OrderId = 704,
                Reason = "Hàng vỡ gãy khi vận chuyển",
                BankName = null,
                AccountNumber = null,
                AccountHolderName = null
            };

            // Act: Khách không truyền số tài khoản ngân hàng
            var result = await service.RequestRefundAsync("customer-with-bank", dto);

            // Assert
            Assert.True(result.Success);
            var refund = await context.RefundRequests.FirstOrDefaultAsync(r => r.OrderId == 704);
            Assert.NotNull(refund);
            Assert.Equal("Vietcombank", refund.BankName);
            Assert.Equal("9943265155", refund.AccountNumber);
            Assert.Equal("NGUYEN HUYNH KHA", refund.AccountHolderName);
        }

        [Fact]
        public async Task RequestRefundAsync_WhenOrderNotPaid_ReturnsFailure()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var order = new Order
            {
                OrderId = 705,
                OrderCode = "OD_CUST_705",
                UserId = "customer-unpaid",
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                FinalAmount = 450000
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            var dto = new CreateRefundRequestDto
            {
                OrderId = 705,
                Reason = "Muốn hoàn tiền đơn chưa thanh toán",
                BankName = "VietinBank",
                AccountNumber = "10101010",
                AccountHolderName = "TEST UNPAID"
            };

            // Act
            var result = await service.RequestRefundAsync("customer-unpaid", dto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("đã thanh toán (Paid)", result.Message);
        }
    }
}
