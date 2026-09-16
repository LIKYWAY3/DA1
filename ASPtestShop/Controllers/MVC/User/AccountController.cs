using ASPtestShop.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Services.Interfaces.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
    using ASPtestShop.Data;
    using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Profile;
using ASPtestShop.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    namespace ASPtestShop.Controllers.MVC
    {
        [Route("account")]
        public class AccountController : Controller
        {
            private readonly IUserAuthService _userAuthService;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IWebHostEnvironment _webHostEnvironment;

            public AccountController(IUserAuthService userAuthService, 
                   UserManager<ApplicationUser> userManager,
                   IWebHostEnvironment webHostEnvironment)
            {
                _userAuthService = userAuthService;
                _userManager = userManager;
                _webHostEnvironment = webHostEnvironment;
            }

            // GET: /account/login
            [HttpGet("login")]
            public IActionResult Login(string? returnUrl = null, bool sessionExpired = false)
            {
                ViewBag.ReturnUrl = returnUrl;
                if (sessionExpired)
                {
                    ViewBag.SessionExpiredMessage = "Tài khoản của bạn vừa được đăng nhập trên một thiết bị khác. Phiên làm việc trên thiết bị này đã được tự động đăng xuất để bảo mật.";
                }
                return View();
            }

            // POST: /account/login
            [HttpPost("login")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
            {
                ViewBag.ReturnUrl = returnUrl;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await _userAuthService.LoginAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                // Single Session: Đổi SecurityStamp mới cho User trong CSDL => Ngay lập tức vô hiệu hóa mọi thiết bị cũ!
                var appUser = await _userManager.FindByIdAsync(result.UserId);
                if (appUser != null)
                {
                    await _userManager.UpdateSecurityStampAsync(appUser);
                    appUser = await _userManager.FindByIdAsync(result.UserId);
                }

                var securityStamp = appUser?.SecurityStamp ?? Guid.NewGuid().ToString();

                var displayName = !string.IsNullOrWhiteSpace(result.FullName)
                    ? result.FullName
                    : result.UserName;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.UserId),
                    new Claim(ClaimTypes.Name, displayName),
                    new Claim(ClaimTypes.Email, result.Email),
                    new Claim(ClaimTypes.Role, "Customer"),
                    new Claim("security_stamp", securityStamp)
                };

                var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(7)
                        : DateTimeOffset.UtcNow.AddHours(6)
                };

                await HttpContext.SignInAsync(
                    UserCookieAuth.Scheme,
                    principal,
                    authProperties
                );

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            // POST: /account/logout
            [HttpPost("logout")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Logout()
            {
                await HttpContext.SignOutAsync(UserCookieAuth.Scheme);
                return RedirectToAction("Index", "Home");
            }

            // GET: /account/logout
            [HttpGet("logout")]
            public async Task<IActionResult> LogoutGet()
            {
                await HttpContext.SignOutAsync(UserCookieAuth.Scheme);
                return RedirectToAction("Index", "Home");
            }

            // GET: /account/access-denied
            [HttpGet("access-denied")]
            public IActionResult AccessDenied()
            {
                return View();
            }

            // GET: /account/register
            [HttpGet("register")]
            public IActionResult Register()
            {
                return View();
            }

            // POST: /account/register
            [HttpPost("register")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Register(RegisterViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await _userAuthService.InitiateRegisterAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                TempData["SuccessMessage"] = result.Message;

                return RedirectToAction("VerifyEmail", new { email = model.Email });
            }

            // GET: /account/verify-email
            [HttpGet("verify-email")]
            public IActionResult VerifyEmail(string? email)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return RedirectToAction("Register");
                }

                var model = new VerifyEmailViewModel
                {
                    Email = email
                };

                return View(model);
            }

            // POST: /account/verify-email
            [HttpPost("verify-email")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await _userAuthService.VerifyOtpAndRegisterAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                TempData["SuccessMessage"] = result.Message;

                return RedirectToAction("Login");
            }

            // POST: /account/resend-otp
            [HttpPost("resend-otp")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ResendOtp([FromForm] string email)
            {
                var result = await _userAuthService.ResendOtpAsync(email);
                return Json(new { success = result.Success, message = result.Message });
            }
        // GET: /account/profile
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return RedirectToAction("Login");

            var userProfile = await _userAuthService.GetUserProfileAsync(userId);

            if (userProfile == null) return NotFound("Không tìm thấy thông tin người dùng");

            var model = new UserProfileViewModel
            {
                Id = userProfile.Id,
                FullName = userProfile.FullName,
                UserName = userProfile.UserName,
                Email = userProfile.Email,
                PhoneNumber = userProfile.PhoneNumber,
                Address = userProfile.Address,
                Gender = userProfile.Gender,
                AvatarUrl = userProfile.AvatarUrl
            };

            return View(model);
        }
        // POST: /account/update-profile
        [HttpPost("update-profile")]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return RedirectToAction("Login");

                var dto = new UpdateProfileDto
                {
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Gender = model.Gender,
                    AvatarFile = model.AvatarFile
                };

                var result = await _userAuthService.UpdateProfileAsync(userId, dto);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message; 
                    return RedirectToAction("Profile");
                }

                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }

                return View("Profile", model);
            }
            // GET: /account/ChangePhone
            [HttpGet("ChangePhone")]
            [Authorize]
            public IActionResult ChangePhone()
            {
                return View();
            }

            // POST: /account/ChangePhone
            [HttpPost("ChangePhone")]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ChangePhone(ChangePhoneViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password!);
                if (!checkPassword)
                {
                    ModelState.AddModelError("Password", "Mật khẩu xác nhận không chính xác.");
                    return View(model);
                }

                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.NewPhone!);
                if (!setPhoneResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật số điện thoại.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Cập nhật số điện thoại thành công!";
                return RedirectToAction("Profile");
            }

            // GET: /account/ChangeEmail
            [HttpGet("ChangeEmail")]
            [Authorize]
            public IActionResult ChangeEmail()
            {
                return View();
            }

            // POST: /account/ChangeEmail
            [HttpPost("ChangeEmail")]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model); 
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password!);
                if (!checkPassword)
                {
                    ModelState.AddModelError("Password", "Mật khẩu xác nhận không chính xác.");
                    return View(model);
                }

                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail!);
                var setOriginalEmailResult = await _userManager.ChangeEmailAsync(user, model.NewEmail!, token);

                if (setOriginalEmailResult.Succeeded)
                {
                    await _userManager.SetUserNameAsync(user, model.NewEmail);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi hoặc Email này đã được sử dụng.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Cập nhật địa chỉ Email thành công!";
                return RedirectToAction("Profile");
            }
            // GET: /account/bank
            [HttpGet("bank")]
            [Authorize]
            public async Task<IActionResult> Bank()
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return RedirectToAction("Login");

                var user = await _userManager.FindByIdAsync(userId);

                var banks = await _userAuthService.GetBankAccountsAsync(userId);

                var model = new UserProfileViewModel
                {
                    UserName = user.UserName,
                    AvatarUrl = user.AvatarUrl,
                    BankAccounts = banks 
                };

                return View(model);
            }

            // POST: /account/add-bank
            [HttpPost("add-bank")]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddBankAccount(AddBankAccountViewModel model)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await _userAuthService.AddBankAccountAsync(userId, model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }

                return RedirectToAction("Bank");
            }
            // GET: /account/address
            [HttpGet("address")]
            [Authorize]
            public async Task<IActionResult> Address()
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return RedirectToAction("Login");

                var user = await _userManager.FindByIdAsync(userId);

                var addresses = await _userAuthService.GetUserAddressesAsync(userId);
                ViewBag.Addresses = addresses;

                var model = new UserProfileViewModel
                {
                    UserName = user.UserName,
                    AvatarUrl = user.AvatarUrl
                };

                return View(model);
            }

            // POST: /account/add-address
            [HttpPost("add-address")]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> AddAddress(AddAddressViewModel model)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return RedirectToAction("Login");

                var result = await _userAuthService.AddAddressAsync(userId, model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }

                return RedirectToAction("Address");
            }

            [HttpPost("set-default-address/{id}")]
            [Authorize]
            public async Task<IActionResult> SetDefaultAddress(int id)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _userAuthService.SetDefaultAddressAsync(userId, id);
                if (result.Success) TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Address");
            }

            [HttpPost("edit-address")]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> EditAddress(EditAddressViewModel model)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _userAuthService.EditAddressAsync(userId, model);
                if (result.Success) TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Address");
            }
            // GET: /account/change-password
            [HttpGet("change-password")]
            public IActionResult ChangePassword()
            {
                return View(new ChangePasswordViewModel());
            }

            // POST: /account/change-password
            [HttpPost("change-password")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
            {
                if (model.CurrentPassword == model.NewPassword)
                {
                    ModelState.AddModelError("NewPassword", "Mật khẩu mới không được giống với mật khẩu hiện tại!");
                }
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await _userAuthService.ChangePasswordAsync(userId, model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message ?? "Có lỗi xảy ra khi đổi mật khẩu.");
                    return View(model);
                }
            }


        // GET: /account/forgot-password
        [HttpGet("forgot-password")]
            public IActionResult ForgotPassword()
            {
                return View();
            }

            // POST: /account/forgot-password
            [HttpPost("forgot-password")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await _userAuthService.ForgotPasswordAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                if (!string.IsNullOrWhiteSpace(result.ResetToken))
                {
                    var resetLink = Url.Action(
                        "ResetPassword",
                        "Account",
                        new
                        {
                            email = result.Email,
                            resetToken = result.ResetToken
                        },
                        Request.Scheme
                    );

                    TempData["ResetLink"] = resetLink;
                }

                TempData["SuccessMessage"] = result.Message;

                return RedirectToAction("ForgotPassword");
            }

            // GET: /account/reset-password
            [HttpGet("reset-password")]
            public IActionResult ResetPassword(string email, string resetToken)
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(resetToken))
                {
                    return RedirectToAction("Login");
                }

                var model = new ResetPasswordViewModel
                {
                    Email = email,
                    ResetToken = resetToken
                };

                return View(model);
            }

            // POST: /account/reset-password
            [HttpPost("reset-password")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await _userAuthService.ResetPasswordAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                TempData["SuccessMessage"] = result.Message;

                return RedirectToAction("Login");
            }
        }
    }