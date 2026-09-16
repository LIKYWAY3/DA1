using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Models.ViewModels.Profile;
using ASPtestShop.Services.Interfaces;
using ASPtestShop.Services.Interfaces.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ASPtestShop.Services.Implementations.User
{
    public class PendingRegistration
    {
        public RegisterViewModel Model { get; set; } = new();
        public string OtpCode { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(5);
    }

    public class UserAuthService : IUserAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public UserAuthService(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            HttpClient httpClient,
            AppDbContext context,
            IMemoryCache cache,
            IEmailService emailService)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _httpClient = httpClient;
            _context = context;
            _cache = cache;
            _emailService = emailService;
        }

        public async Task<UserLoginResultDto> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.EmailOrUserName);

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUserName);
            }

            if (user == null)
            {
                return new UserLoginResultDto
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }

            var passwordOk = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!passwordOk)
            {
                return new UserLoginResultDto
                {
                    Success = false,
                    Message = "Tài khoản hoặc mật khẩu không đúng"
                };
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                return new UserLoginResultDto
                {
                    Success = false,
                    Message = "Tài khoản Admin vui lòng đăng nhập tại trang quản trị"
                };
            }

            return new UserLoginResultDto
            {
                Success = true,
                Message = "Đăng nhập thành công",
                UserId = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FullName = user.FullName ?? ""
            };
        }

        public async Task<UserRegisterResultDto> RegisterAsync(RegisterViewModel model)
        {
            var emailExists = await _userManager.FindByEmailAsync(model.Email ?? "");

            if (emailExists != null)
            {
                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = "Email này đã được sử dụng"
                };
            }

            var usernameExists = await _userManager.FindByNameAsync(model.UserName ?? "");

            if (usernameExists != null)
            {
                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = "Username này đã được sử dụng"
                };
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address,
                Gender = model.Gender,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, model.Password ?? "");

            if (!createResult.Succeeded)
            {
                var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));

                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = errors
                };
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            // Tạo 01 Cart rỗng liên kết UserId (Theo PTTK BR-03)
            var existingCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (existingCart == null)
            {
                _context.Carts.Add(new Cart { UserId = user.Id });
                await _context.SaveChangesAsync();
            }

            return new UserRegisterResultDto
            {
                Success = true,
                Message = "Đăng ký tài khoản thành công"
            };
        }

        public async Task<UserRegisterResultDto> InitiateRegisterAsync(RegisterViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return new UserRegisterResultDto { Success = false, Message = "Email không được để trống." };
            }

            var emailExists = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (emailExists != null)
            {
                return new UserRegisterResultDto { Success = false, Message = "Email này đã được sử dụng." };
            }

            var usernameExists = await _userManager.FindByNameAsync(model.UserName?.Trim() ?? "");
            if (usernameExists != null)
            {
                return new UserRegisterResultDto { Success = false, Message = "Username này đã được sử dụng." };
            }

            // Sinh mã OTP 6 chữ số
            var otpCode = Random.Shared.Next(100000, 999999).ToString();

            var pending = new PendingRegistration
            {
                Model = model,
                OtpCode = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            var cacheKey = $"PendingReg_{model.Email.Trim().ToLower()}";
            _cache.Set(cacheKey, pending, TimeSpan.FromMinutes(5));

            // Soạn email OTP HTML
            var subject = "[FuuFishing] Mã xác thực đăng ký tài khoản";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 580px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #3b5d50; color: #ffffff; padding: 24px; text-align: center;'>
                        <h2 style='margin: 0;'>FuuFishing Store</h2>
                        <p style='margin: 6px 0 0 0; opacity: 0.9; font-size: 14px;'>Đồ câu & Phụ kiện dã ngoại chuyên nghiệp</p>
                    </div>
                    <div style='padding: 28px; color: #333333; line-height: 1.6;'>
                        <p>Xin chào <strong>{model.FullName ?? model.UserName}</strong>,</p>
                        <p>Cảm ơn bạn đã đăng ký tài khoản tại FuuFishing. Để hoàn tất quy trình kích hoạt tài khoản, vui lòng nhập mã xác thực (OTP) dưới đây:</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <div style='display: inline-block; background-color: #f3f8f5; border: 2px dashed #3b5d50; border-radius: 8px; padding: 14px 32px;'>
                                <span style='font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #253f36;'>{otpCode}</span>
                            </div>
                            <p style='color: #888888; font-size: 13px; margin-top: 8px;'>Mã xác thực có hiệu lực trong vòng <strong>5 phút</strong>.</p>
                        </div>
                        
                        <p style='font-size: 14px; color: #666;'>Nếu bạn không thực hiện yêu cầu đăng ký này, vui lòng bỏ qua email này hoặc liên hệ hỗ trợ.</p>
                    </div>
                    <div style='background-color: #f8f9fa; padding: 16px; text-align: center; font-size: 12px; color: #888;'>
                        FuuFishing &copy; {DateTime.Now.Year} - Nền tảng Đồ câu chuyên nghiệp
                    </div>
                </div>";

            var (sendSuccess, sendMsg) = await _emailService.SendEmailAsync(model.Email.Trim(), subject, body);

            return new UserRegisterResultDto
            {
                Success = true,
                Message = sendSuccess
                    ? "Mã xác thực OTP đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư."
                    : $"Mã xác thực đã được tạo (Thông báo hệ thống: {sendMsg})."
            };
        }

        public Task<string?> GetPendingOtpCodeAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return Task.FromResult<string?>(null);
            var cacheKey = $"PendingReg_{email.Trim().ToLower()}";
            if (_cache.TryGetValue(cacheKey, out PendingRegistration? pending) && pending != null)
            {
                return Task.FromResult<string?>(pending.OtpCode);
            }
            return Task.FromResult<string?>(null);
        }

        public async Task<UserRegisterResultDto> VerifyOtpAndRegisterAsync(VerifyEmailViewModel model)
        {
            var cacheKey = $"PendingReg_{model.Email.Trim().ToLower()}";
            if (!_cache.TryGetValue(cacheKey, out PendingRegistration? pending) || pending == null)
            {
                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = "Mã xác thực đã hết hạn hoặc không tồn tại. Vui lòng đăng ký lại."
                };
            }

            if (DateTime.UtcNow > pending.ExpiresAt)
            {
                _cache.Remove(cacheKey);
                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = "Mã xác thực đã hết hạn (quá 5 phút). Vui lòng nhấn gửi lại mã."
                };
            }

            if (pending.OtpCode != model.OtpCode.Trim())
            {
                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = "Mã xác thực không chính xác. Vui lòng kiểm tra lại hộp thư."
                };
            }

            // Tạo tài khoản chính thức
            var user = new ApplicationUser
            {
                UserName = pending.Model.UserName,
                Email = pending.Model.Email,
                FullName = pending.Model.FullName,
                Address = pending.Model.Address,
                Gender = pending.Model.Gender,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, pending.Model.Password ?? "");
            if (!createResult.Succeeded)
            {
                var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                return new UserRegisterResultDto { Success = false, Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            // Tự động tạo 01 Cart rỗng liên kết UserId (Theo PTTK BR-03)
            var existingCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (existingCart == null)
            {
                _context.Carts.Add(new Cart { UserId = user.Id });
                await _context.SaveChangesAsync();
            }

            _cache.Remove(cacheKey);

            return new UserRegisterResultDto
            {
                Success = true,
                Message = "Xác minh email thành công! Tài khoản của bạn đã được kích hoạt."
            };
        }

        public async Task<UserRegisterResultDto> ResendOtpAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new UserRegisterResultDto { Success = false, Message = "Email không hợp lệ." };
            }

            var cacheKey = $"PendingReg_{email.Trim().ToLower()}";
            if (!_cache.TryGetValue(cacheKey, out PendingRegistration? pending) || pending == null)
            {
                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = "Phiên đăng ký không còn tồn tại hoặc đã quá hạn. Vui lòng đăng ký lại từ đầu."
                };
            }

            // Cooldown 60 giây
            var elapsed = DateTime.UtcNow - pending.CreatedAt;
            if (elapsed < TimeSpan.FromSeconds(60))
            {
                var waitSec = 60 - (int)elapsed.TotalSeconds;
                return new UserRegisterResultDto
                {
                    Success = false,
                    Message = $"Vui lòng đợi thêm {waitSec} giây trước khi yêu cầu gửi lại mã."
                };
            }

            // Tạo mã mới
            var newOtp = Random.Shared.Next(100000, 999999).ToString();
            pending.OtpCode = newOtp;
            pending.CreatedAt = DateTime.UtcNow;
            pending.ExpiresAt = DateTime.UtcNow.AddMinutes(5);

            _cache.Set(cacheKey, pending, TimeSpan.FromMinutes(5));

            var subject = "[FuuFishing] Mã xác thực đăng ký tài khoản (Gửi lại)";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 580px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #3b5d50; color: #ffffff; padding: 24px; text-align: center;'>
                        <h2 style='margin: 0;'>FuuFishing Store</h2>
                        <p style='margin: 6px 0 0 0; opacity: 0.9; font-size: 14px;'>Đồ câu & Phụ kiện dã ngoại chuyên nghiệp</p>
                    </div>
                    <div style='padding: 28px; color: #333333; line-height: 1.6;'>
                        <p>Xin chào <strong>{pending.Model.FullName ?? pending.Model.UserName}</strong>,</p>
                        <p>Bạn vừa yêu cầu gửi lại mã xác thực cho tài khoản FuuFishing:</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <div style='display: inline-block; background-color: #f3f8f5; border: 2px dashed #3b5d50; border-radius: 8px; padding: 14px 32px;'>
                                <span style='font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #253f36;'>{newOtp}</span>
                            </div>
                            <p style='color: #888888; font-size: 13px; margin-top: 8px;'>Mã xác thực có hiệu lực trong vòng <strong>5 phút</strong>.</p>
                        </div>
                    </div>
                    <div style='background-color: #f8f9fa; padding: 16px; text-align: center; font-size: 12px; color: #888;'>
                        FuuFishing &copy; {DateTime.Now.Year} - Nền tảng Đồ câu chuyên nghiệp
                    </div>
                </div>";

            var (sendSuccess, sendMsg) = await _emailService.SendEmailAsync(email.Trim(), subject, body);

            return new UserRegisterResultDto
            {
                Success = true,
                Message = sendSuccess
                    ? "Đã gửi lại mã xác thực mới vào hộp thư của bạn."
                    : $"Đã tạo mã OTP mới (Thông báo hệ thống: {sendMsg})."
            };
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var defaultAddress = await _context.UserAddresses
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsDefault);

            string addressDisplay = defaultAddress != null ? defaultAddress.SpecificAddress : "Chưa thiết lập";

            return new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = addressDisplay,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl
            };
        }
        public async Task<AuthResultDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Message = "Không tìm thấy người dùng!" };
            }

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.Gender = dto.Gender;

            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "Avatars");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.AvatarFile.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.AvatarFile.CopyToAsync(fileStream);
                }

                user.AvatarUrl = "/Uploads/Avatars/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new AuthResultDto { Success = true, Message = "Cập nhật hồ sơ thành công!" };
            }

            return new AuthResultDto
            {
                Success = false,
                Message = "Cập nhật thất bại",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }
        public async Task<List<BankAccountDto>> GetBankAccountsAsync(string userId)
        {
            try
            {
                string apiUrl = $"https://api.yourdomain.com/v1/banks/{userId}";

                var banks = await _httpClient.GetFromJsonAsync<List<BankAccountDto>>(apiUrl);

                return banks ?? new List<BankAccountDto>();
            }
            catch (Exception)
            {
                return new List<BankAccountDto>();
            }
        }

        public async Task<AuthResultDto> AddBankAccountAsync(string userId, AddBankAccountViewModel model)
        {
            try
            {
                string apiUrl = $"https://api.yourdomain.com/v1/banks/add";

                var payload = new
                {
                    UserId = userId,
                    BankName = model.BankName,
                    AccountName = model.AccountName.ToUpper(),
                    AccountNumber = model.AccountNumber
                };

                var response = await _httpClient.PostAsJsonAsync(apiUrl, payload);

                if (response.IsSuccessStatusCode)
                {
                    return new AuthResultDto { Success = true, Message = "Thêm tài khoản ngân hàng thành công!" };
                }

                return new AuthResultDto { Success = false, Message = "API từ chối yêu cầu." };
            }
            catch (Exception ex)
            {
                return new AuthResultDto { Success = false, Message = "Lỗi kết nối đến Server API." };
            }
        }
        public async Task<List<UserAddress>> GetUserAddressesAsync(string userId)
        {
            return await _context.UserAddresses
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsDefault)
                .ToListAsync();
        }

        public async Task<AuthResultDto> AddAddressAsync(string userId, AddAddressViewModel model)
        {
            if (model.IsDefault)
            {
                var currentDefaults = await _context.UserAddresses
                    .Where(x => x.UserId == userId && x.IsDefault)
                    .ToListAsync();

                foreach (var addr in currentDefaults)
                {
                    addr.IsDefault = false;
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.PhoneNumber = model.PhoneNumber;
                    await _userManager.UpdateAsync(user);
                }
            }

            var newAddress = new UserAddress
            {
                UserId = userId,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                SpecificAddress = model.SpecificAddress,
                IsDefault = model.IsDefault
            };

            _context.UserAddresses.Add(newAddress);
            await _context.SaveChangesAsync();

            return new AuthResultDto { Success = true, Message = "Thêm địa chỉ mới thành công!" };
        }
        public async Task<AuthResultDto> SetDefaultAddressAsync(string userId, int addressId)
        {
            var addresses = await _context.UserAddresses.Where(x => x.UserId == userId).ToListAsync();
            var targetAddress = addresses.FirstOrDefault(x => x.Id == addressId);

            if (targetAddress == null) return new AuthResultDto { Success = false, Message = "Không tìm thấy địa chỉ" };

            foreach (var addr in addresses) { addr.IsDefault = false; }
            targetAddress.IsDefault = true;

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.PhoneNumber = targetAddress.PhoneNumber; 
                await _userManager.UpdateAsync(user);        
            }

            await _context.SaveChangesAsync();
            return new AuthResultDto { Success = true, Message = "Đã thiết lập làm địa chỉ mặc định!" };
        }

        public async Task<AuthResultDto> EditAddressAsync(string userId, EditAddressViewModel model)
        {
            var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == userId);
            if (address == null) return new AuthResultDto { Success = false, Message = "Không tìm thấy địa chỉ" };

            if (model.IsDefault && !address.IsDefault)
            {
                var currentDefaults = await _context.UserAddresses.Where(x => x.UserId == userId && x.IsDefault).ToListAsync();
                foreach (var addr in currentDefaults) { addr.IsDefault = false; }
            }

            address.FullName = model.FullName;
            address.PhoneNumber = model.PhoneNumber;
            address.SpecificAddress = model.SpecificAddress;

            if (!address.IsDefault || model.IsDefault)
            {
                address.IsDefault = model.IsDefault;
            }

            if (address.IsDefault)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.PhoneNumber = model.PhoneNumber; 
                    await _userManager.UpdateAsync(user);
                }
            }

            await _context.SaveChangesAsync();
            return new AuthResultDto { Success = true, Message = "Cập nhật địa chỉ thành công!" };
        }
        public async Task<AuthResultDto> ChangePasswordAsync(string userId, ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Message = "Không tìm thấy người dùng." };
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Mật khẩu hiện tại không chính xác hoặc không hợp lệ."
                };
            }

            return new AuthResultDto { Success = true, Message = "Đổi mật khẩu thành công!" };
        }

        public async Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new ForgotPasswordResultDto
                {
                    Success = true,
                    Message = "Nếu email tồn tại trong hệ thống, link đặt lại mật khẩu sẽ được tạo."
                };
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return new ForgotPasswordResultDto
            {
                Success = true,
                Message = "Đã tạo link đặt lại mật khẩu.",
                Email = user.Email ?? model.Email,
                ResetToken = resetToken
            };
        }

        public async Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new ResetPasswordResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy tài khoản"
                };
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.ResetToken,
                model.NewPassword
            );

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));

                return new ResetPasswordResultDto
                {
                    Success = false,
                    Message = errors
                };
            }

            await _userManager.UpdateSecurityStampAsync(user);

            return new ResetPasswordResultDto
            {
                Success = true,
                Message = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại."
            };
        }
    }
}