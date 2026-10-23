using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using ASPtestShop.Auth;
using ASPtestShop.Controllers.MVC;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Implementations.User;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class AutoLoginAfterRegistrationTests
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
        public async Task VerifyOtpAndRegisterAsync_ShouldReturnUserDetails_ForAutoLogin()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            userManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser, string>((u, p) => u.Id = "new-user-id-123");
            userManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"))
                .ReturnsAsync(IdentityResult.Success);

            var cache = new MemoryCache(new MemoryCacheOptions());
            var pending = new PendingRegistration
            {
                Model = new RegisterViewModel
                {
                    UserName = "tester01",
                    Email = "tester01@gmail.com",
                    FullName = "Tester Zero One",
                    Password = "Password@123"
                },
                OtpCode = "654321",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                FailedAttempts = 0
            };
            cache.Set("PendingReg_tester01@gmail.com", pending, TimeSpan.FromMinutes(5));

            var service = new UserAuthService(userManager.Object, null!, new HttpClient(), context, cache, null!);

            // Act
            var result = await service.VerifyOtpAndRegisterAsync(new VerifyEmailViewModel
            {
                Email = "tester01@gmail.com",
                OtpCode = "654321"
            });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("new-user-id-123", result.UserId);
            Assert.Equal("tester01", result.UserName);
            Assert.Equal("tester01@gmail.com", result.Email);
            Assert.Equal("Tester Zero One", result.FullName);
        }
    }
}
