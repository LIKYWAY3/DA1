using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Models.DTO.Refund;
using ASPtestShop.Services.Implementations;
using ASPtestShop.Services.Implementations.Admin;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ASPtestShop.Tests
{
    public class AdminBatchOperationsTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new AppDbContext(options);
        }

        // ==========================================
        // 1. COUPON BATCH TESTS
        // ==========================================
        [Fact]
        public async Task BatchToggleCoupons_ShouldActivateAndDeactivateCorrectly()
        {
            using var context = CreateInMemoryDbContext();

            var c1 = new Coupon
            {
                CouponId = 11,
                Code = "TEST11",
                DiscountType = "Percentage",
                DiscountValue = 10,
                IsActive = false,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10)
            };
            var c2 = new Coupon
            {
                CouponId = 12,
                Code = "TEST12",
                DiscountType = "Percentage",
                DiscountValue = 15,
                IsActive = false,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            context.Coupons.AddRange(c1, c2);
            await context.SaveChangesAsync();

            var service = new AdminCouponService(context);

            // Batch Activate
            var activateResult = await service.BatchToggleActiveAsync(new List<int> { 11, 12 }, true);
            Assert.True(activateResult.Success);
            Assert.Equal(2, activateResult.SuccessCount);

            var dbC1 = await context.Coupons.FindAsync(11);
            var dbC2 = await context.Coupons.FindAsync(12);
            Assert.True(dbC1?.IsActive);
            Assert.True(dbC2?.IsActive);

            // Batch Deactivate
            var deactivateResult = await service.BatchToggleActiveAsync(new List<int> { 11 }, false);
            Assert.True(deactivateResult.Success);
            Assert.False(context.Coupons.Find(11)?.IsActive);
        }

        [Fact]
        public async Task BatchDeleteCoupons_ShouldHardDeleteUnusedAndSoftDeleteUsed()
        {
            using var context = CreateInMemoryDbContext();

            var unusedCoupon = new Coupon
            {
                CouponId = 21,
                Code = "UNUSED21",
                DiscountType = "Percentage",
                DiscountValue = 10,
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            var usedCoupon = new Coupon
            {
                CouponId = 22,
                Code = "USED22",
                DiscountType = "Percentage",
                DiscountValue = 20,
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                Orders = new List<Order>
                {
                    new Order { OrderId = 991, OrderCode = "OD_991", OrderStatus = "Completed", PaymentStatus = "Paid" }
                }
            };

            context.Coupons.AddRange(unusedCoupon, usedCoupon);
            await context.SaveChangesAsync();

            var service = new AdminCouponService(context);
            var result = await service.BatchDeleteCouponsAsync(new List<int> { 21, 22 });

            Assert.True(result.Success);
            Assert.Equal(2, result.SuccessCount);

            // Unused should be completely deleted
            Assert.Null(await context.Coupons.FindAsync(21));

            // Used should be soft-deleted (IsActive = false)
            var dbUsed = await context.Coupons.FindAsync(22);
            Assert.NotNull(dbUsed);
            Assert.False(dbUsed.IsActive);
        }

        // ==========================================
        // 2. REVIEW BATCH TESTS
        // ==========================================
        [Fact]
        public async Task BatchApproveAndRejectReviews_ShouldUpdateIsApproved()
        {
            using var context = CreateInMemoryDbContext();

            var r1 = new Review { ReviewId = 31, ProductId = 1, Rating = 5, Comment = "Rất tốt", IsApproved = false };
            var r2 = new Review { ReviewId = 32, ProductId = 1, Rating = 4, Comment = "Khá hài lòng", IsApproved = false };
            var r3 = new Review { ReviewId = 33, ProductId = 1, Rating = 1, Comment = "Spam comment", IsApproved = true };

            context.Reviews.AddRange(r1, r2, r3);
            await context.SaveChangesAsync();

            var service = new AdminReviewService(context);

            // Batch Approve r1, r2
            var approveResult = await service.BatchApproveReviewsAsync(new List<int> { 31, 32 });
            Assert.True(approveResult.Success);
            Assert.Equal(2, approveResult.SuccessCount);
            Assert.True(context.Reviews.Find(31)?.IsApproved);
            Assert.True(context.Reviews.Find(32)?.IsApproved);

            // Batch Reject r3
            var rejectResult = await service.BatchRejectReviewsAsync(new List<int> { 33 });
            Assert.True(rejectResult.Success);
            Assert.False(context.Reviews.Find(33)?.IsApproved);

            // Batch Delete
            var deleteResult = await service.BatchDeleteReviewsAsync(new List<int> { 31, 32, 33 });
            Assert.True(deleteResult.Success);
            Assert.Equal(3, deleteResult.SuccessCount);
            Assert.Null(await context.Reviews.FindAsync(31));
            Assert.Null(await context.Reviews.FindAsync(32));
            Assert.Null(await context.Reviews.FindAsync(33));
        }

        // ==========================================
        // 3. REFUND BATCH TESTS
        // ==========================================
        [Fact]
        public async Task BatchProcessRefunds_ShouldApproveAndRejectCorrectly()
        {
            using var context = CreateInMemoryDbContext();

            var order1 = new Order { OrderId = 801, OrderCode = "OD_801", OrderStatus = "Pending", PaymentStatus = "Paid" };
            var order2 = new Order { OrderId = 802, OrderCode = "OD_802", OrderStatus = "Pending", PaymentStatus = "Paid" };

            var rf1 = new RefundRequest
            {
                RefundRequestId = 41,
                OrderId = 801,
                UserId = "user-801",
                Amount = 250000,
                Reason = "Đổi ý",
                Status = "Pending"
            };

            var rf2 = new RefundRequest
            {
                RefundRequestId = 42,
                OrderId = 802,
                UserId = "user-802",
                Amount = 400000,
                Reason = "Không muốn mua nữa",
                Status = "Pending"
            };

            context.Orders.AddRange(order1, order2);
            context.RefundRequests.AddRange(rf1, rf2);
            await context.SaveChangesAsync();

            var service = new RefundService(context);

            // Batch Approve
            var result = await service.BatchProcessRefundsAsync(
                new List<int> { 41, 42 },
                "AdminUser",
                new ProcessRefundDto
                {
                    Status = "Approved",
                    TransactionCode = "TX-BATCH-001",
                    AdminNote = "Batch approved for testing"
                }
            );

            Assert.True(result.Success);
            Assert.Equal(2, result.SuccessCount);

            var dbRf1 = await context.RefundRequests.FindAsync(41);
            var dbRf2 = await context.RefundRequests.FindAsync(42);

            Assert.Equal("Approved", dbRf1?.Status);
            Assert.Equal("Approved", dbRf2?.Status);
            Assert.Contains("TX-BATCH-001", dbRf1?.AdminNote);

            // Order payment status updated to Refunded
            var dbO1 = await context.Orders.FindAsync(801);
            Assert.Equal("Refunded", dbO1?.PaymentStatus);
            Assert.Equal("Cancelled", dbO1?.OrderStatus);
        }
    }
}
