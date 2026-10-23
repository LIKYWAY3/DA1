using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Implementations;
using ASPtestShop.Services.Implementations.User;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class AccountSynchronizationTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new AppDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager(AppDbContext context)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            mgr.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
            mgr.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            mgr.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser, string>((u, p) =>
                {
                    if (string.IsNullOrEmpty(u.Id)) u.Id = Guid.NewGuid().ToString();
                });

            mgr.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        [Fact]
        public async Task ApiRegister_ShouldAssignCustomerRole_AndCreateEmptyCart()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var userManager = MockUserManager(context);
            var config = new Mock<IConfiguration>();

            var authService = new AuthService(userManager.Object, config.Object, context);
            var dto = new RegisterDto
            {
                UserName = "apicustomer",
                Email = "api@customer.com",
                FullName = "API Customer",
                PhoneNumber = "0987654321",
                Password = "Password@123"
            };

            // Act
            var result = await authService.RegisterAsync(dto);

            // Assert
            Assert.True(result.Success);
            userManager.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"), Times.Once, "API Register must assign Customer role!");

            var cart = await context.Carts.FirstOrDefaultAsync();
            Assert.NotNull(cart);
            Assert.False(string.IsNullOrEmpty(cart.UserId), "BR-03: Cart must be created and linked to the new user!");
        }

        [Fact]
        public async Task MvcCoreRegister_ShouldAssignCustomerRole_AndCreateEmptyCart()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var userManager = MockUserManager(context);
            var env = new Mock<IWebHostEnvironment>();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var emailService = new Mock<IEmailService>();

            var userAuthService = new UserAuthService(userManager.Object, env.Object, new HttpClient(), context, cache, emailService.Object);

            // Act
            var result = await userAuthService.CreateCustomerAccountWithCartAsync(
                "mvccustomer",
                "mvc@customer.com",
                "MVC Customer",
                "0912345678",
                "123 Street",
                "Nam",
                "Password@123"
            );

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.UserId);
            userManager.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Customer"), Times.Once);

            var cart = await context.Carts.FirstOrDefaultAsync(c => c.UserId == result.UserId);
            Assert.NotNull(cart);
        }
    }
}
