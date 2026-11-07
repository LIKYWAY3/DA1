using System.Security.Claims;
using ASPtestShop.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.MVC.Admin
{
    [Route("admin")]
    public class AdminAuthController : Controller
    {
        private readonly IAdminAuthService _adminAuthService;

        public AdminAuthController(IAdminAuthService adminAuthService)
        {
            _adminAuthService = adminAuthService;
        }

        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? "/admin" });
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? "/admin" });
            }

            var result = await _adminAuthService.LoginAsync(model);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Login", "Account", new { returnUrl = returnUrl ?? "/admin" });
            }

            var userManager = HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ASPtestShop.Data.ApplicationUser>>();
            var appUser = await userManager.FindByIdAsync(result.UserId);
            if (appUser != null)
            {
                await userManager.UpdateSecurityStampAsync(appUser);
                appUser = await userManager.FindByIdAsync(result.UserId);
            }
            var stamp = appUser?.SecurityStamp ?? Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId),
                new Claim(ClaimTypes.Name, result.UserName),
                new Claim(ClaimTypes.Email, result.Email),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("security_stamp", stamp ?? "")
            };

            var identity = new ClaimsIdentity(claims, AdminCookieAuth.Scheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(6)
            };

            await HttpContext.SignInAsync(
                AdminCookieAuth.Scheme,
                principal,
                authProperties
            );

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/admin");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(AdminCookieAuth.Scheme);
            await HttpContext.SignOutAsync(UserCookieAuth.Scheme);
            return Redirect("/");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(AdminCookieAuth.Scheme);
            await HttpContext.SignOutAsync(UserCookieAuth.Scheme);
            return Redirect("/");
        }

        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("AccessDenied", "Account");
        }
    }
}
