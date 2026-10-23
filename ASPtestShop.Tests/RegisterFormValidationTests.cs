using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ASPtestShop.Controllers.MVC;
using ASPtestShop.Data;
using ASPtestShop.Models.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class RegisterFormValidationTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }

        [Theory]
        [InlineData("123456")] // No uppercase, no special char
        [InlineData("password")] // No uppercase, no digit, no special char
        [InlineData("PASSWORD123")] // No lowercase, no special char
        [InlineData("Pass12")] // Less than 6 chars
        public void Password_WithoutComplexity_ShouldFailValidation(string weakPassword)
        {
            var model = new RegisterViewModel
            {
                UserName = "validuser",
                Email = "valid@example.com",
                FullName = "Valid User",
                PhoneNumber = "0987654321",
                Password = weakPassword,
                ConfirmPassword = weakPassword
            };

            var errors = ValidateModel(model);
            Assert.Contains(errors, e => e.MemberNames.Contains("Password"));
        }

        [Theory]
        [InlineData("123")] // Too short
        [InlineData("012345678912")] // 12 digits
        [InlineData("0287654321")] // Invalid carrier prefix
        [InlineData("0987abcdef")] // Letters
        public void PhoneNumber_WithInvalidFormat_ShouldFailValidation(string invalidPhone)
        {
            var model = new RegisterViewModel
            {
                UserName = "validuser",
                Email = "valid@example.com",
                FullName = "Valid User",
                PhoneNumber = invalidPhone,
                Password = "Password@123",
                ConfirmPassword = "Password@123"
            };

            var errors = ValidateModel(model);
            Assert.Contains(errors, e => e.MemberNames.Contains("PhoneNumber"));
        }

        [Theory]
        [InlineData("0987654321")] // Viettel
        [InlineData("0387654321")] // Viettel
        [InlineData("0787654321")] // Mobifone
        [InlineData("0887654321")] // Vinaphone
        [InlineData("0587654321")] // Vietnamobile
        public void PhoneNumber_WithValidVietnameseFormat_ShouldPassValidation(string validPhone)
        {
            var model = new RegisterViewModel
            {
                UserName = "validuser",
                Email = "valid@example.com",
                FullName = "Valid User",
                PhoneNumber = validPhone,
                Password = "Password@123",
                ConfirmPassword = "Password@123"
            };

            var errors = ValidateModel(model);
            Assert.DoesNotContain(errors, e => e.MemberNames.Contains("PhoneNumber"));
        }

        [Fact]
        public async Task CheckAvailability_ShouldReturnJsonStatus()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            userManager.Setup(m => m.FindByNameAsync("taken_user"))
                .ReturnsAsync(new ApplicationUser { UserName = "taken_user" });
            userManager.Setup(m => m.FindByNameAsync("free_user"))
                .ReturnsAsync((ApplicationUser?)null);

            userManager.Setup(m => m.FindByEmailAsync("taken@gmail.com"))
                .ReturnsAsync(new ApplicationUser { Email = "taken@gmail.com" });
            userManager.Setup(m => m.FindByEmailAsync("free@gmail.com"))
                .ReturnsAsync((ApplicationUser?)null);

            var controller = new AccountController(null!, userManager.Object, null!, null!);

            // Act
            var result = await controller.CheckAvailability("taken_user", "free@gmail.com") as JsonResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}
