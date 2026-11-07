using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using ASPtestShop.Auth;
using ASPtestShop.Controllers.Api;
using ASPtestShop.Controllers.MVC;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Implementations.User;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class AccountLockAndSessionTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new AppDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        [Fact]
        public async Task LoginAsync_WhenUserIsLockedOut_ReturnsFailureWithAdminLockedMessage()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var userManager = MockUserManager();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var emailService = new Mock<IEmailService>();
            var env = new Mock<IWebHostEnvironment>();

            var lockedUser = new ApplicationUser
            {
                Id = "locked-user-1",
                UserName = "lockeduser",
                Email = "locked@test.com",
                FullName = "Locked User",
                LockoutEnabled = true,
                LockoutEnd = DateTimeOffset.UtcNow.AddYears(10)
            };

            userManager.Setup(m => m.FindByEmailAsync("locked@test.com"))
                .ReturnsAsync(lockedUser);
            userManager.Setup(m => m.CheckPasswordAsync(lockedUser, "P@ssword123"))
                .ReturnsAsync(true);
            userManager.Setup(m => m.GetRolesAsync(lockedUser))
                .ReturnsAsync(new List<string> { "Customer" });

            var service = new UserAuthService(
                userManager.Object,
                env.Object,
                new System.Net.Http.HttpClient(),
                context,
                memoryCache,
                emailService.Object
            );

            var model = new LoginViewModel
            {
                EmailOrUserName = "locked@test.com",
                Password = "P@ssword123"
            };

            // Act
            var result = await service.LoginAsync(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Tài khoản của bạn đã bị khóa bởi quản trị viên. Vui lòng liên hệ hỗ trợ.", result.Message);
        }

        [Fact]
        public void AccountController_Login_WhenAccountLockedQueryParam_SetsAccountLockedMessage()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Captcha:SiteKey"]).Returns("dummy-key");

            var controller = new AccountController(
                null!, null!, null!, null!, null!, configMock.Object, null!, null!
            );

            // Act
            var result = controller.Login(returnUrl: "/profile", sessionExpired: false, accountLocked: true);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Tài khoản của bạn đã bị khóa bởi quản trị viên. Vui lòng liên hệ quản trị viên để được hỗ trợ.", controller.ViewBag.AccountLockedMessage);
            Assert.Null(controller.ViewBag.SessionExpiredMessage);
        }

        [Fact]
        public void AccountController_Login_WhenSessionExpiredQueryParam_SetsSessionExpiredMessage()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Captcha:SiteKey"]).Returns("dummy-key");

            var controller = new AccountController(
                null!, null!, null!, null!, null!, configMock.Object, null!, null!
            );

            // Act
            var result = controller.Login(returnUrl: null, sessionExpired: true, accountLocked: false);

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal("Tài khoản của bạn vừa được đăng nhập trên một thiết bị khác. Phiên làm việc trên thiết bị này đã được tự động đăng xuất để bảo mật.", controller.ViewBag.SessionExpiredMessage);
            Assert.Null(controller.ViewBag.AccountLockedMessage);
        }

        [Fact]
        public async Task AuthApiController_CheckSession_WhenUserIsLocked_ReturnsAccountLockedReason()
        {
            // Arrange
            var userManager = MockUserManager();
            var lockedUser = new ApplicationUser
            {
                Id = "user-locked-api",
                UserName = "userlocked",
                SecurityStamp = "current-stamp",
                LockoutEnd = DateTimeOffset.UtcNow.AddHours(2)
            };

            userManager.Setup(m => m.FindByIdAsync("user-locked-api"))
                .ReturnsAsync(lockedUser);

            var authServiceMock = new Mock<IAuthenticationService>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);
            serviceProviderMock
                .Setup(s => s.GetService(typeof(UserManager<ApplicationUser>)))
                .Returns(userManager.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user-locked-api"),
                new Claim("security_stamp", "current-stamp")
            };
            var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
                User = principal
            };

            var controller = new AuthApiController(null!)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Act
            var actionResult = await controller.CheckSession();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var json = JsonSerializer.Serialize(okResult.Value);
            using var doc = JsonDocument.Parse(json);
            Assert.True(doc.RootElement.GetProperty("isAuthenticated").GetBoolean());
            Assert.False(doc.RootElement.GetProperty("valid").GetBoolean());
            Assert.Equal("account_locked", doc.RootElement.GetProperty("reason").GetString());
        }

        [Fact]
        public async Task AuthApiController_CheckSession_WhenSecurityStampMismatch_ReturnsSessionExpiredReason()
        {
            // Arrange
            var userManager = MockUserManager();
            var activeUser = new ApplicationUser
            {
                Id = "user-mismatch-api",
                UserName = "usermismatch",
                SecurityStamp = "new-stamp-from-other-device",
                LockoutEnd = null
            };

            userManager.Setup(m => m.FindByIdAsync("user-mismatch-api"))
                .ReturnsAsync(activeUser);

            var authServiceMock = new Mock<IAuthenticationService>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);
            serviceProviderMock
                .Setup(s => s.GetService(typeof(UserManager<ApplicationUser>)))
                .Returns(userManager.Object);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user-mismatch-api"),
                new Claim("security_stamp", "old-stamp-on-this-device")
            };
            var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
                User = principal
            };

            var controller = new AuthApiController(null!)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Act
            var actionResult = await controller.CheckSession();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var json = JsonSerializer.Serialize(okResult.Value);
            using var doc = JsonDocument.Parse(json);
            Assert.True(doc.RootElement.GetProperty("isAuthenticated").GetBoolean());
            Assert.False(doc.RootElement.GetProperty("valid").GetBoolean());
            Assert.Equal("session_expired", doc.RootElement.GetProperty("reason").GetString());
        }
    }
}
