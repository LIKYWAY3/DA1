using System.Security.Claims;
using ASPtestShop.Auth;
using ASPtestShop.Models.ViewModels.Profile;
using ASPtestShop.Services.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/user/bank-accounts")]
    [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme)]
    public class UserBankAccountApiController : ControllerBase
    {
        private readonly IUserAuthService _userAuthService;

        public UserBankAccountApiController(IUserAuthService userAuthService)
        {
            _userAuthService = userAuthService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBankAccounts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bạn chưa đăng nhập." });
            }

            var banks = await _userAuthService.GetBankAccountsAsync(userId);
            return Ok(banks);
        }

        [HttpPost]
        public async Task<IActionResult> AddBankAccount([FromBody] AddBankAccountViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bạn chưa đăng nhập." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = string.Join(", ", errors) });
            }

            var result = await _userAuthService.AddBankAccountAsync(userId, model);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { success = true, message = result.Message });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBankAccount(int id, [FromBody] AddBankAccountViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bạn chưa đăng nhập." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = string.Join(", ", errors) });
            }

            var result = await _userAuthService.UpdateBankAccountAsync(userId, id, model);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { success = true, message = result.Message });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBankAccount(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bạn chưa đăng nhập." });
            }

            var result = await _userAuthService.DeleteBankAccountAsync(userId, id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { success = true, message = result.Message });
        }

        [HttpPatch("{id:int}/default")]
        public async Task<IActionResult> SetDefaultBankAccount(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Bạn chưa đăng nhập." });
            }

            var result = await _userAuthService.SetDefaultBankAccountAsync(userId, id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { success = true, message = result.Message });
        }
    }
}
