using ASPtestShop.Auth;
using ASPtestShop.Services.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/user")]
    [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
    public class UserAddressApiController : ControllerBase
    {
        private readonly IUserAuthService _userAuthService;

        public UserAddressApiController(IUserAuthService userAuthService)
        {
            _userAuthService = userAuthService;
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bạn chưa đăng nhập." });
            }

            var addresses = await _userAuthService.GetUserAddressesAsync(userId);

            var result = addresses.Select(a => new
            {
                id = a.Id,
                fullName = a.FullName,
                phoneNumber = a.PhoneNumber,
                specificAddress = a.SpecificAddress,
                isDefault = a.IsDefault
            }).ToList();

            return Ok(result);
        }

        [HttpGet("profile-info")]
        public async Task<IActionResult> GetProfileInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bạn chưa đăng nhập." });
            }

            var profile = await _userAuthService.GetUserProfileAsync(userId);

            if (profile == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin tài khoản." });
            }

            return Ok(new
            {
                fullName = profile.FullName,
                phoneNumber = profile.PhoneNumber,
                email = profile.Email
            });
        }
    }
}
