using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using ASPtestShop.Data;
using ASPtestShop.Data.Entities;

namespace ASPtestShop.Controllers.Api
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthApiController(IAuthService authService)
        {
            _authService = authService;
        }

        //===================================REGISTER=========================================
        // POST: api/auth/register
        // Đăng ký người dùng mới
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message,
                    result.Errors
                });
            }

            return Ok(new
            {
                result.Message
            });
        }

        //===================================LOGIN=========================================
        // POST: api/auth/login
        // Đăng nhập và nhận JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
            {
                return Unauthorized(new
                {
                    result.Message
                });
            }

            return Ok(new
            {
                result.Message,
                token = result.Token
            });
        }

        //====================================PROFILE=========================================
        // GET: api/auth/profile
        // API được bảo vệ bằng JWT và Cookie
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme + "," + ASPtestShop.Auth.UserCookieAuth.Scheme)]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            // Lấy userId từ token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Lấy userName từ token
            var userName = User.Identity?.Name;

            var profile = _authService.GetProfile(userId, userName);

            return Ok(new
            {
                message = profile.Message,
                userId = profile.UserId,
                userName = profile.UserName
            });
        }

        //====================================FORGOT PASSWORD=========================================
        // POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);

            return Ok(new
            {
                result.Message,
                resetToken = result.ResetToken
            });
        }

        //====================================RESET PASSWORD=========================================
        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message,
                    result.Errors
                });
            }

            return Ok(new
            {
                result.Message
            });
        }

        //=====================================LOGOUT=========================================
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var result = await _authService.LogoutAsync(userId);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(new
            {
                result.Message
            });
        }

        //==================================CHECK SESSION=======================================
        // GET: api/auth/check-session
        // Kiểm tra phiên đăng nhập và SecurityStamp (Single Session Heartbeat)
        [HttpGet("check-session")]
        public async Task<IActionResult> CheckSession()
        {
            if (HttpContext.Items.ContainsKey("ForceLogout"))
            {
                return Ok(new { isAuthenticated = true, valid = false, reason = "session_expired" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cookieStamp = User.FindFirstValue("security_stamp");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(cookieStamp))
            {
                return Ok(new { isAuthenticated = false, valid = false, reason = "anonymous" });
            }

            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(userId);

            if (user == null || user.SecurityStamp != cookieStamp || (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow))
            {
                await HttpContext.SignOutAsync(ASPtestShop.Auth.UserCookieAuth.Scheme);
                return Ok(new { isAuthenticated = true, valid = false, reason = "session_expired" });
            }

            return Ok(new { isAuthenticated = true, valid = true });
        }
    }
}