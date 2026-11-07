using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ASPtestShop.Auth;
using ASPtestShop.Controllers.Api;
using ASPtestShop.Controllers.MVC.User;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.Interfaces.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class UserAddressAndCheckoutTests
    {
        [Fact]
        public async Task GetProfileInfo_WhenUserNotAuthenticated_ShouldReturnUnauthorized()
        {
            // Arrange
            var userAuthServiceMock = new Mock<IUserAuthService>();
            var controller = new UserAddressApiController(userAuthServiceMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.GetProfileInfo();

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task GetProfileInfo_WhenProfileHasNoPhone_ShouldFallbackToDefaultAddressPhone()
        {
            // Arrange
            var userId = "user-123";
            var userAuthServiceMock = new Mock<IUserAuthService>();

            userAuthServiceMock
                .Setup(s => s.GetUserProfileAsync(userId))
                .ReturnsAsync(new UserProfileDto
                {
                    Id = userId,
                    FullName = "Nguyễn Văn A",
                    PhoneNumber = null,
                    Email = "vana@example.com"
                });

            userAuthServiceMock
                .Setup(s => s.GetUserAddressesAsync(userId))
                .ReturnsAsync(new List<UserAddress>
                {
                    new UserAddress
                    {
                        Id = 1,
                        FullName = "Nguyễn Văn A (Nhận)",
                        PhoneNumber = "0987654321",
                        SpecificAddress = "Số 123 đường ABC, Phường X, Quận Y, Hà Nội",
                        IsDefault = true
                    }
                });

            var controller = new UserAddressApiController(userAuthServiceMock.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            // Act
            var result = await controller.GetProfileInfo() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            dynamic data = result.Value!;
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            Assert.Equal("Nguyễn Văn A", root.GetProperty("fullName").GetString());
            Assert.Equal("0987654321", root.GetProperty("phoneNumber").GetString());
            Assert.Equal("vana@example.com", root.GetProperty("email").GetString());
        }

        [Fact]
        public async Task GetProfileInfo_WhenProfileHasPhone_ShouldPreferProfilePhone()
        {
            // Arrange
            var userId = "user-456";
            var userAuthServiceMock = new Mock<IUserAuthService>();

            userAuthServiceMock
                .Setup(s => s.GetUserProfileAsync(userId))
                .ReturnsAsync(new UserProfileDto
                {
                    Id = userId,
                    FullName = "Trần Thị B",
                    PhoneNumber = "0911223344",
                    Email = "thib@example.com"
                });

            userAuthServiceMock
                .Setup(s => s.GetUserAddressesAsync(userId))
                .ReturnsAsync(new List<UserAddress>
                {
                    new UserAddress
                    {
                        Id = 2,
                        FullName = "Trần Thị B",
                        PhoneNumber = "0988776655",
                        SpecificAddress = "Số 456 đường DEF",
                        IsDefault = true
                    }
                });

            var controller = new UserAddressApiController(userAuthServiceMock.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            // Act
            var result = await controller.GetProfileInfo() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            dynamic data = result.Value!;
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            Assert.Equal("Trần Thị B", root.GetProperty("fullName").GetString());
            Assert.Equal("0911223344", root.GetProperty("phoneNumber").GetString());
        }

        [Fact]
        public async Task GetAddresses_WhenUserHasAddresses_ShouldReturnFormattedList()
        {
            // Arrange
            var userId = "user-789";
            var userAuthServiceMock = new Mock<IUserAuthService>();

            userAuthServiceMock
                .Setup(s => s.GetUserAddressesAsync(userId))
                .ReturnsAsync(new List<UserAddress>
                {
                    new UserAddress
                    {
                        Id = 10,
                        FullName = "Lê Văn C",
                        PhoneNumber = "0900112233",
                        SpecificAddress = "Số 10 đường XYZ",
                        IsDefault = true
                    },
                    new UserAddress
                    {
                        Id = 11,
                        FullName = "Lê Văn C (Nhà riêng)",
                        PhoneNumber = "0900112244",
                        SpecificAddress = "Số 20 đường XYZ",
                        IsDefault = false
                    }
                });

            var controller = new UserAddressApiController(userAuthServiceMock.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            // Act
            var result = await controller.GetAddresses() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var json = System.Text.Json.JsonSerializer.Serialize(result.Value);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            Assert.Equal(2, root.GetArrayLength());
            Assert.Equal(10, root[0].GetProperty("id").GetInt32());
            Assert.True(root[0].GetProperty("isDefault").GetBoolean());
            Assert.Equal(11, root[1].GetProperty("id").GetInt32());
            Assert.False(root[1].GetProperty("isDefault").GetBoolean());
        }

        [Fact]
        public async Task CheckoutIndex_WhenUnauthenticated_ReturnsChallengeResult()
        {
            // Arrange
            var cartServiceMock = new Mock<ICartService>();
            var controller = new CheckoutController(cartServiceMock.Object, null!, null!, null!)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.Index();

            // Assert
            var challenge = Assert.IsType<ChallengeResult>(result);
            Assert.Contains(UserCookieAuth.Scheme, challenge.AuthenticationSchemes);
        }

        [Fact]
        public async Task CheckoutIndex_WhenCartIsEmpty_RedirectsToCart()
        {
            // Arrange
            var userId = "user-empty-cart";
            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock
                .Setup(c => c.HasCartItemsAsync(userId))
                .ReturnsAsync(false);

            var controller = new CheckoutController(cartServiceMock.Object, null!, null!, null!)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Cart", redirect.ControllerName);
        }

        [Fact]
        public async Task CheckoutIndex_WhenCartHasItems_ReturnsViewResult()
        {
            // Arrange
            var userId = "user-has-cart";
            var cartServiceMock = new Mock<ICartService>();
            cartServiceMock
                .Setup(c => c.HasCartItemsAsync(userId))
                .ReturnsAsync(true);

            var controller = new CheckoutController(cartServiceMock.Object, null!, null!, null!)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
