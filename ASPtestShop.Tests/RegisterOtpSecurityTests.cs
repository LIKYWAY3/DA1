using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Implementations.User;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class RegisterOtpSecurityTests
    {
        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            mgr.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            mgr.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            return mgr;
        }

        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task InitiateRegisterAsync_ShouldReturnFalse_WhenEmailSendingFails()
        {
            // Arrange
            var userManager = MockUserManager();
            var env = new Mock<IWebHostEnvironment>();
            using var context = CreateInMemoryDbContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var emailService = new Mock<IEmailService>();

            emailService.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((false, "Lỗi xác thực SMTP: 535 Bad Credentials"));

            var service = new UserAuthService(userManager.Object, env.Object, new HttpClient(), context, cache, emailService.Object);

            var model = new RegisterViewModel
            {
                UserName = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                Password = "Password@123",
                ConfirmPassword = "Password@123"
            };

            // Act
            var result = await service.InitiateRegisterAsync(model);

            // Assert
            Assert.False(result.Success, "InitiateRegisterAsync must return Success = false when email delivery fails!");
            Assert.Contains("Lỗi", result.Message);
        }

        [Fact]
        public async Task VerifyOtp_ShouldLockAndRemoveCache_WhenWrongAttemptsExceedFive()
        {
            // Arrange
            var userManager = MockUserManager();
            var env = new Mock<IWebHostEnvironment>();
            using var context = CreateInMemoryDbContext();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var emailService = new Mock<IEmailService>();

            var pending = new PendingRegistration
            {
                Model = new RegisterViewModel
                {
                    UserName = "testuser",
                    Email = "test@example.com",
                    FullName = "Test User",
                    Password = "Password@123"
                },
                OtpCode = "123456",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                FailedAttempts = 0
            };
            cache.Set("PendingReg_test@example.com", pending, TimeSpan.FromMinutes(5));

            var service = new UserAuthService(userManager.Object, env.Object, new HttpClient(), context, cache, emailService.Object);

            // Act - Enter wrong OTP 5 times
            UserRegisterResultDto? lastResult = null;
            for (int i = 0; i < 5; i++)
            {
                lastResult = await service.VerifyOtpAndRegisterAsync(new VerifyEmailViewModel
                {
                    Email = "test@example.com",
                    OtpCode = "999999" // Wrong OTP
                });
            }

            // Assert
            Assert.NotNull(lastResult);
            Assert.False(lastResult.Success);
            Assert.Contains("5 lần", lastResult.Message);

            // Cache should now be removed / locked
            var exists = cache.TryGetValue("PendingReg_test@example.com", out _);
            Assert.False(exists, "Pending registration must be removed from cache after 5 failed attempts!");
        }
    }
}
