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
using System.Text.Json;
    namespace ASPtestShop.Controllers.MVC
    {
        [Route("account")]
        public class AccountController : Controller
        {
            private readonly IUserAuthService _userAuthService;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IWebHostEnvironment _webHostEnvironment;
            private readonly IEmailService _emailService;
            private readonly ICaptchaService _captchaService;
            private readonly IConfiguration _configuration;
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly AppDbContext _context;

            public AccountController(IUserAuthService userAuthService, 
                   UserManager<ApplicationUser> userManager,
                   IWebHostEnvironment webHostEnvironment,
                   IEmailService emailService,
                   ICaptchaService captchaService,
                   IConfiguration configuration,
                   IHttpClientFactory httpClientFactory,
                   AppDbContext context)
            {
                _userAuthService = userAuthService;
                _userManager = userManager;
                _webHostEnvironment = webHostEnvironment;
                _emailService = emailService;
                _captchaService = captchaService;
                _configuration = configuration;
                _httpClientFactory = httpClientFactory;
                _context = context;
            }

            // GET: /account/login
            [HttpGet("login")]
            public IActionResult Login(string? returnUrl = null, bool sessionExpired = false, bool accountLocked = false)
            {
                ViewBag.ReturnUrl = returnUrl;
                var siteKey = _configuration["Captcha:SiteKey"];
                if (string.IsNullOrWhiteSpace(siteKey) || siteKey.StartsWith("YOUR_")) { siteKey = "1x00000000000000000000AA"; }
                ViewBag.TurnstileSiteKey = siteKey;
                if (accountLocked)
                {
                    ViewBag.AccountLockedMessage = "Tài khoản của bạn đã bị khóa bởi quản trị viên. Vui lòng liên hệ quản trị viên để được hỗ trợ.";
                }
                else if (sessionExpired)
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
                var siteKey = _configuration["Captcha:SiteKey"];
                if (string.IsNullOrWhiteSpace(siteKey) || siteKey.StartsWith("YOUR_")) { siteKey = "1x00000000000000000000AA"; }
                ViewBag.TurnstileSiteKey = siteKey;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // 2.1. Xác minh CAPTCHA Backend
                string? turnstileResponse = Request.Form["cf-turnstile-response"];
                var isCaptchaValid = await _captchaService.VerifyAsync(turnstileResponse ?? "");
                if (!isCaptchaValid)
                {
                    ModelState.AddModelError("", "Xác minh CAPTCHA thất bại hoặc đã hết hạn. Vui lòng thử lại.");
                    return View(model);
                }

                var result = await _userAuthService.LoginAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                var appUser = await _userManager.FindByIdAsync(result.UserId);
                if (appUser != null)
                {
                    await _userManager.UpdateSecurityStampAsync(appUser);
                    appUser = await _userManager.FindByIdAsync(result.UserId);
                }

                var securityStamp = appUser?.SecurityStamp ?? Guid.NewGuid().ToString();
                var displayName = !string.IsNullOrWhiteSpace(result.FullName) ? result.FullName : result.UserName;

                if (result.IsAdmin)
                {
                    var adminClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.UserId),
                        new Claim(ClaimTypes.Name, displayName),
                        new Claim(ClaimTypes.Email, result.Email),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim("security_stamp", securityStamp)
                    };

                    var adminIdentity = new ClaimsIdentity(adminClaims, AdminCookieAuth.Scheme);
                    var adminPrincipal = new ClaimsPrincipal(adminIdentity);

                    await HttpContext.SignInAsync(
                        AdminCookieAuth.Scheme,
                        adminPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(6)
                        }
                    );

                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
                    {
                        return Redirect(returnUrl);
                    }

                    return Redirect("/admin");
                }
                else
                {
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.UserId),
                        new Claim(ClaimTypes.Name, displayName),
                        new Claim(ClaimTypes.Email, result.Email),
                        new Claim(ClaimTypes.Role, "Customer"),
                        new Claim("security_stamp", securityStamp)
                    };

                    var userIdentity = new ClaimsIdentity(userClaims, UserCookieAuth.Scheme);
                    var userPrincipal = new ClaimsPrincipal(userIdentity);

                    await HttpContext.SignInAsync(
                        UserCookieAuth.Scheme,
                        userPrincipal,
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(6)
                        }
                    );

                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) && !returnUrl.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            // 2.2. ĐĂNG NHẬP BẰNG GOOGLE (OAuth 2.0 / OpenID Connect - Phương án B)
            [HttpGet("google-login")]
            public IActionResult GoogleLogin(string? returnUrl = null)
            {
                var clientId = _configuration["Authentication:Google:ClientId"];
                if (string.IsNullOrWhiteSpace(clientId) || clientId.StartsWith("YOUR_"))
                {
                    TempData["ErrorMessage"] = "Chức năng đăng nhập Google chưa được cấu hình Google ClientId.";
                    return RedirectToAction("Login", new { returnUrl });
                }

                var redirectUri = Url.Action("GoogleCallback", "Account", null, Request.Scheme);
                var state = returnUrl ?? "/";

                var googleAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                                   $"client_id={Uri.EscapeDataString(clientId)}&" +
                                   $"redirect_uri={Uri.EscapeDataString(redirectUri ?? "")}&" +
                                   $"response_type=code&" +
                                   $"scope=openid%20email%20profile&" +
                                   $"state={Uri.EscapeDataString(state)}&" +
                                   $"prompt=select_account";

                return Redirect(googleAuthUrl);
            }

            // GET: /account/google-callback
            [HttpGet("google-callback")]
            public async Task<IActionResult> GoogleCallback(string? code, string? error, string? state = null)
            {
                var returnUrl = state;

                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(code))
                {
                    TempData["ErrorMessage"] = "Bạn đã hủy cấp quyền hoặc đăng nhập Google thất bại.";
                    return RedirectToAction("Login", new { returnUrl });
                }

                var clientId = _configuration["Authentication:Google:ClientId"];
                var clientSecret = _configuration["Authentication:Google:ClientSecret"];
                var redirectUri = Url.Action("GoogleCallback", "Account", null, Request.Scheme);

                try
                {
                    var client = _httpClientFactory.CreateClient();

                    var tokenRequest = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("code", code),
                        new KeyValuePair<string, string>("client_id", clientId ?? ""),
                        new KeyValuePair<string, string>("client_secret", clientSecret ?? ""),
                        new KeyValuePair<string, string>("redirect_uri", redirectUri ?? ""),
                        new KeyValuePair<string, string>("grant_type", "authorization_code")
                    });

                    var tokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
                    if (!tokenResponse.IsSuccessStatusCode)
                    {
                        var errResp = await tokenResponse.Content.ReadAsStringAsync();
                        TempData["ErrorMessage"] = $"Xác thực mã Google thất bại ({tokenResponse.StatusCode}): {errResp}";
                        return RedirectToAction("Login", new { returnUrl });
                    }

                    var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                    using var tokenDoc = JsonDocument.Parse(tokenJson);
                    if (!tokenDoc.RootElement.TryGetProperty("access_token", out var accessTokenProp))
                    {
                        TempData["ErrorMessage"] = "Google không trả về Access Token hợp lệ.";
                        return RedirectToAction("Login", new { returnUrl });
                    }

                    var accessToken = accessTokenProp.GetString();

                    var requestMsg = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
                    requestMsg.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    var userResponse = await client.SendAsync(requestMsg);
                    if (!userResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Không thể lấy thông tin tài khoản Google.";
                        return RedirectToAction("Login", new { returnUrl });
                    }

                    var userJson = await userResponse.Content.ReadAsStringAsync();
                    using var userDoc = JsonDocument.Parse(userJson);

                    var email = userDoc.RootElement.TryGetProperty("email", out var em) ? em.GetString() : null;
                    var verifiedEmail = userDoc.RootElement.TryGetProperty("verified_email", out var ve) && ve.GetBoolean();
                    var name = userDoc.RootElement.TryGetProperty("name", out var nm) ? nm.GetString() : "Người dùng Google";
                    var picture = userDoc.RootElement.TryGetProperty("picture", out var pic) ? pic.GetString() : null;

                    if (string.IsNullOrEmpty(email) || !verifiedEmail)
                    {
                        TempData["ErrorMessage"] = "Tài khoản Google của bạn chưa được xác minh email.";
                        return RedirectToAction("Login", new { returnUrl });
                    }

                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        var baseUserName = email.Split('@')[0].Replace(".", "_");
                        var uniqueSuffix = Guid.NewGuid().ToString("N")[..6];
                        var userName = $"{baseUserName}_{uniqueSuffix}";

                        user = new ApplicationUser
                        {
                            UserName = userName,
                            Email = email,
                            EmailConfirmed = true,
                            FullName = name,
                            AvatarUrl = picture
                        };

                        var createResult = await _userManager.CreateAsync(user);
                        if (!createResult.Succeeded)
                        {
                            TempData["ErrorMessage"] = "Không thể khởi tạo tài khoản mới từ Google: " + string.Join(", ", createResult.Errors.Select(e => e.Description));
                            return RedirectToAction("Login", new { returnUrl });
                        }

                        await _userManager.AddToRoleAsync(user, "Customer");

                        var cart = new ASPtestShop.Data.Entities.Cart
                        {
                            UserId = user.Id,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.Carts.Add(cart);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                        {
                            return RedirectToAction("Login", new { returnUrl, accountLocked = true });
                        }

                        if (string.IsNullOrEmpty(user.AvatarUrl) && !string.IsNullOrEmpty(picture))
                        {
                            user.AvatarUrl = picture;
                            await _userManager.UpdateAsync(user);
                        }
                    }

                    await _userManager.UpdateSecurityStampAsync(user);
                    user = await _userManager.FindByIdAsync(user.Id);
                    var stamp = user?.SecurityStamp ?? Guid.NewGuid().ToString();

                    var userRoles = await _userManager.GetRolesAsync(user!);
                    var isAdmin = userRoles.Contains("Admin");

                    if (isAdmin)
                    {
                        var adminClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user!.Id),
                            new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? "Admin"),
                            new Claim(ClaimTypes.Email, user.Email ?? ""),
                            new Claim(ClaimTypes.Role, "Admin"),
                            new Claim("security_stamp", stamp)
                        };

                        var adminIdentity = new ClaimsIdentity(adminClaims, AdminCookieAuth.Scheme);
                        var adminPrincipal = new ClaimsPrincipal(adminIdentity);

                        await HttpContext.SignInAsync(
                            AdminCookieAuth.Scheme,
                            adminPrincipal,
                            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(6) }
                        );

                        return Redirect("/admin");
                    }
                    else
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user!.Id),
                            new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? "Customer"),
                            new Claim(ClaimTypes.Email, user.Email ?? ""),
                            new Claim(ClaimTypes.Role, "Customer"),
                            new Claim("security_stamp", stamp)
                        };

                        var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(
                            UserCookieAuth.Scheme,
                            principal,
                            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) }
                        );

                        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) && !returnUrl.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi đăng nhập bằng Google: " + ex.Message;
                    return RedirectToAction("Login", new { returnUrl });
                }
            }

            // POST: /account/logout
            [HttpPost("logout")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Logout()
            {
                await HttpContext.SignOutAsync(UserCookieAuth.Scheme);
                await HttpContext.SignOutAsync(AdminCookieAuth.Scheme);
                return RedirectToAction("Index", "Home");
            }

            // GET: /account/logout
            [HttpGet("logout")]
            public async Task<IActionResult> LogoutGet()
            {
                await HttpContext.SignOutAsync(UserCookieAuth.Scheme);
                await HttpContext.SignOutAsync(AdminCookieAuth.Scheme);
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
                var siteKey = _configuration["Captcha:SiteKey"];
                if (string.IsNullOrWhiteSpace(siteKey) || siteKey.StartsWith("YOUR_")) { siteKey = "1x00000000000000000000AA"; }
                ViewBag.TurnstileSiteKey = siteKey;
                return View();
            }

            // POST: /account/register (Phương án 2: Đăng ký trực tiếp có CAPTCHA, không cần OTP)
            [HttpPost("register")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Register(RegisterViewModel model)
            {
                var siteKey = _configuration["Captcha:SiteKey"];
                if (string.IsNullOrWhiteSpace(siteKey) || siteKey.StartsWith("YOUR_")) { siteKey = "1x00000000000000000000AA"; }
                ViewBag.TurnstileSiteKey = siteKey;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string? turnstileResponse = Request.Form["cf-turnstile-response"];
                var isCaptchaValid = await _captchaService.VerifyAsync(turnstileResponse ?? "");
                if (!isCaptchaValid)
                {
                    ModelState.AddModelError("", "Xác minh CAPTCHA thất bại hoặc đã hết hạn. Vui lòng xác minh lại.");
                    return View(model);
                }

                var result = await _userAuthService.RegisterAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }

                var appUser = await _userManager.FindByIdAsync(result.UserId);
                if (appUser != null)
                {
                    await _userManager.UpdateSecurityStampAsync(appUser);
                    appUser = await _userManager.FindByIdAsync(result.UserId);
                }

                var securityStamp = appUser?.SecurityStamp ?? Guid.NewGuid().ToString();
                var displayName = !string.IsNullOrWhiteSpace(result.FullName) ? result.FullName : result.UserName;

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

                await HttpContext.SignInAsync(
                    UserCookieAuth.Scheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    }
                );

                TempData["SuccessMessage"] = $"Đăng ký tài khoản thành công! Chào mừng {displayName} đến với FuuFishing.";
                return RedirectToAction("Index", "Home");
            }

            // GET: /account/check-availability
            [HttpGet("check-availability")]
            public async Task<IActionResult> CheckAvailability([FromQuery] string? userName, [FromQuery] string? email)
            {
                bool userNameAvailable = true;
                bool emailAvailable = true;

                if (!string.IsNullOrWhiteSpace(userName))
                {
                    var existingUser = await _userManager.FindByNameAsync(userName.Trim());
                    userNameAvailable = (existingUser == null);
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    var existingEmail = await _userManager.FindByEmailAsync(email.Trim());
                    emailAvailable = (existingEmail == null);
                }

                return Json(new
                {
                    userNameAvailable,
                    emailAvailable
                });
            }

            // GET: /account/verify-email
            [HttpGet("verify-email")]
            public async Task<IActionResult> VerifyEmail(string? email)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return RedirectToAction("Register");
                }

                var model = new VerifyEmailViewModel
                {
                    Email = email
                };

                var emailSettings = await _emailService.GetSettingsAsync();
                ViewBag.IsEmailConfigured = emailSettings.IsConfigured;
                if (_webHostEnvironment.IsDevelopment())
                {
                    ViewBag.DevOtpCode = await _userAuthService.GetPendingOtpCodeAsync(model.Email);
                }

                return View(model);
            }

            // POST: /account/verify-email
            [HttpPost("verify-email")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    var emailSettings = await _emailService.GetSettingsAsync();
                    ViewBag.IsEmailConfigured = emailSettings.IsConfigured;
                if (_webHostEnvironment.IsDevelopment())
                {
                    ViewBag.DevOtpCode = await _userAuthService.GetPendingOtpCodeAsync(model.Email);
                }
                    return View(model);
                }

                var result = await _userAuthService.VerifyOtpAndRegisterAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.Message);
                    var emailSettings = await _emailService.GetSettingsAsync();
                    ViewBag.IsEmailConfigured = emailSettings.IsConfigured;
                if (_webHostEnvironment.IsDevelopment())
                {
                    ViewBag.DevOtpCode = await _userAuthService.GetPendingOtpCodeAsync(model.Email);
                }
                    return View(model);
                }

                var user = !string.IsNullOrEmpty(result.UserId) ? await _userManager.FindByIdAsync(result.UserId) : null;
                var securityStamp = user?.SecurityStamp ?? Guid.NewGuid().ToString();
                var displayName = !string.IsNullOrWhiteSpace(result.FullName) ? result.FullName : (result.UserName ?? "Khách hàng");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.UserId ?? ""),
                    new Claim(ClaimTypes.Name, displayName),
                    new Claim(ClaimTypes.Email, result.Email ?? model.Email),
                    new Claim(ClaimTypes.Role, "Customer"),
                    new Claim("security_stamp", securityStamp)
                };

                var identity = new ClaimsIdentity(claims, UserCookieAuth.Scheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    UserCookieAuth.Scheme,
                    principal,
                    authProperties
                );

                TempData["SuccessMessage"] = $"Đăng ký và kích hoạt tài khoản thành công! Chào mừng {displayName} gia nhập FuuFishing.";

                return RedirectToAction("Index", "Home");
            }

            // POST: /account/resend-otp
            [HttpPost("resend-otp")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ResendOtp([FromForm] string email)
            {
                var result = await _userAuthService.ResendOtpAsync(email);
                var emailSettings = await _emailService.GetSettingsAsync();
                return Json(new { 
                    success = result.Success, 
                    message = result.Message,
                    isConfigured = emailSettings.IsConfigured
                });
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

            [HttpPost("delete-address/{id}")]
            [Authorize]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteAddress(int id)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return RedirectToAction("Login");

                var result = await _userAuthService.DeleteAddressAsync(userId, id);
                if (result.Success)
                    TempData["SuccessMessage"] = result.Message;
                else
                    TempData["ErrorMessage"] = result.Message;

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