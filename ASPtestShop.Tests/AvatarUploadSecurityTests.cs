using System.IO;
using System.Text;
using System.Threading.Tasks;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Services.Implementations.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace ASPtestShop.Tests
{
    public class AvatarUploadSecurityTests
    {
        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            return mgr;
        }

        private IFormFile CreateMockFormFile(string fileName, byte[] content)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "AvatarFile", fileName);
        }

        [Theory]
        [InlineData("malicious_shell.php")]
        [InlineData("exploit.exe")]
        [InlineData("script.sh")]
        [InlineData("hack.html")]
        public async Task UpdateProfile_ShouldRejectDisallowedExtensions(string dangerousFileName)
        {
            var userManager = MockUserManager();
            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            var user = new ApplicationUser { Id = "test-user-id", UserName = "tester" };
            userManager.Setup(m => m.FindByIdAsync("test-user-id")).ReturnsAsync(user);

            var service = new UserAuthService(userManager.Object, env.Object, null!, null!, null!, null!);

            var formFile = CreateMockFormFile(dangerousFileName, Encoding.UTF8.GetBytes("echo 'pwned';"));
            var dto = new UpdateProfileDto { AvatarFile = formFile };

            var result = await service.UpdateProfileAsync("test-user-id", dto);

            Assert.False(result.Success);
            Assert.Contains("Chỉ cho phép upload file ảnh", result.Message);
        }

        [Fact]
        public async Task UpdateProfile_ShouldRejectFileExceeding5MB()
        {
            var userManager = MockUserManager();
            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            var user = new ApplicationUser { Id = "test-user-id", UserName = "tester" };
            userManager.Setup(m => m.FindByIdAsync("test-user-id")).ReturnsAsync(user);

            var service = new UserAuthService(userManager.Object, env.Object, null!, null!, null!, null!);

            // 6MB content
            var largeContent = new byte[6 * 1024 * 1024];
            var formFile = CreateMockFormFile("avatar_too_big.png", largeContent);
            var dto = new UpdateProfileDto { AvatarFile = formFile };

            var result = await service.UpdateProfileAsync("test-user-id", dto);

            Assert.False(result.Success);
            Assert.Contains("không được vượt quá 5MB", result.Message);
        }
    }
}
