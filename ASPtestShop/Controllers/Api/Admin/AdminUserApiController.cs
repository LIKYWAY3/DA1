using System.Security.Claims;
using ASPtestShop.Auth;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(AuthenticationSchemes = AdminCookieAuth.Scheme, Roles = "Admin")]
    public class AdminUserApiController : ControllerBase
    {
        private readonly IAdminUserService _userService;

        public AdminUserApiController(IAdminUserService userService)
        {
            _userService = userService;
        }

        // GET: /api/admin/users?search=...
        [HttpGet("")]
        public async Task<IActionResult> GetUsers([FromQuery] string? search = null)
        {
            var list = await _userService.GetUsersAsync(search);
            return Ok(list);
        }

        // POST: /api/admin/users/{userId}/toggle-lock
        [HttpPost("{userId}/toggle-lock")]
        public async Task<IActionResult> ToggleLock(string userId)
        {
            var currentAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var result = await _userService.ToggleLockUserAsync(currentAdminId, userId);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message, isLockedOut = result.IsLockedOut });
        }
    }
}
