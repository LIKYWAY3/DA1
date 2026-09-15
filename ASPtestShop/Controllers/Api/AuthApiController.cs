using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    }
}