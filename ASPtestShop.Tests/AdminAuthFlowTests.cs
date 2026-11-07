using System;
using System.Threading.Tasks;
using ASPtestShop.Auth;
using ASPtestShop.Controllers.MVC.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class AdminAuthFlowTests
    {
        [Fact]
        public void AdminLoginGet_ShouldRedirectToAccountLogin_WithReturnUrl()
        {
            // Arrange
            var controller = new AdminAuthController(null!);

            // Act
            var result = controller.Login(null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
            Assert.Equal("/admin", result.RouteValues?["returnUrl"]);
        }

        [Fact]
        public void AdminLoginGet_WithCustomReturnUrl_ShouldPreserveReturnUrl()
        {
            // Arrange
            var controller = new AdminAuthController(null!);

            // Act
            var result = controller.Login("/admin/products") as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
            Assert.Equal("/admin/products", result.RouteValues?["returnUrl"]);
        }

        [Fact]
        public async Task AdminLogoutPost_ShouldSignOutBothSchemes_AndRedirectToHome()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContext = new DefaultHttpContext { RequestServices = serviceProviderMock.Object };
            var controller = new AdminAuthController(null!)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Act
            var result = await controller.Logout() as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("/", result.Url);

            authServiceMock.Verify(a => a.SignOutAsync(httpContext, AdminCookieAuth.Scheme, It.IsAny<AuthenticationProperties?>()), Times.Once);
            authServiceMock.Verify(a => a.SignOutAsync(httpContext, UserCookieAuth.Scheme, It.IsAny<AuthenticationProperties?>()), Times.Once);
        }

        [Fact]
        public async Task AdminLogoutGet_ShouldSignOutBothSchemes_AndRedirectToHome()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContext = new DefaultHttpContext { RequestServices = serviceProviderMock.Object };
            var controller = new AdminAuthController(null!)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Act
            var result = await controller.LogoutGet() as RedirectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("/", result.Url);

            authServiceMock.Verify(a => a.SignOutAsync(httpContext, AdminCookieAuth.Scheme, It.IsAny<AuthenticationProperties?>()), Times.Once);
            authServiceMock.Verify(a => a.SignOutAsync(httpContext, UserCookieAuth.Scheme, It.IsAny<AuthenticationProperties?>()), Times.Once);
        }

        [Fact]
        public void AdminAccessDenied_ShouldRedirectToAccountAccessDenied()
        {
            // Arrange
            var controller = new AdminAuthController(null!);

            // Act
            var result = controller.AccessDenied() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AccessDenied", result.ActionName);
            Assert.Equal("Account", result.ControllerName);
        }
    }
}
