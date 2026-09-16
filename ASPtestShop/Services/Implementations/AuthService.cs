using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASPtestShop.Services.Implementations
{
    // AuthService chứa toàn bộ logic xử lý đăng ký, đăng nhập, tạo JWT
    // Controller không dùng UserManager hoặc IConfiguration trực tiếp nữa
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        //=================================Register========================================
        // Đăng ký người dùng mới
        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            // Tạo đối tượng ApplicationUser từ dữ liệu client gửi lên
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName,
                Address = dto.Address, 
                Gender = dto.Gender
            };

            // UserManager sẽ tự hash password và lưu user vào bảng AspNetUsers
            var result = await _userManager.CreateAsync(user, dto.Password);

            // Nếu đăng ký thất bại thì trả danh sách lỗi
            if (!result.Succeeded)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Đăng ký thất bại",
                    Errors = result.Errors
                        .Select(e => e.Description)
                        .ToList()
                };
            }

            return new AuthResultDto
            {
                Success = true,
                Message = "Đăng ký thành công"
            };
        }

        //=================================Login========================================
        // Đăng nhập người dùng
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            // Tìm user theo UserName
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                };
            }

            // Kiểm tra password
            var checkPassword = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!checkPassword)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng"
                };
            }

            // Single Session: Cập nhật SecurityStamp mới để vô hiệu hóa token ở thiết bị cũ
            await _userManager.UpdateSecurityStampAsync(user);

            // Tạo JWT token
            var token = await GenerateJwtTokenAsync(user);

            return new AuthResultDto
            {
                Success = true,
                Message = "Đăng nhập thành công",
                Token = token
            };
        }

        //=================================GetProfile========================================
        // Lấy thông tin profile từ claims
        public ProfileResultDto GetProfile(string? userId, string? userName)
        {
            return new ProfileResultDto
            {
                Message = "Token hợp lệ",
                UserId = userId,
                UserName = userName
            };
        }

        //===============================GenerateJwtToken========================================
        // Hàm riêng để tạo JWT token
        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            // Lấy SecurityStamp hiện tại của user
            var securityStamp = await _userManager.GetSecurityStampAsync(user);

            // Claims là thông tin sẽ được lưu trong token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("security_stamp", securityStamp) //Check SecurityStamp token còn hợp lệ hay không
            };
            //=============================================================================
            // Lấy danh sách role của user
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            //=============================================================================
            // Lấy key từ appsettings.json
            var jwtKey = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception("JWT Key chưa được cấu hình trong appsettings.json");
            }

            // Tạo key để ký token
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            // Tạo chữ ký cho token
            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            // Tạo token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            // Chuyển token object thành chuỗi token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //=================================ForgotPassword========================================
        public async Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                // Không nói rõ email không tồn tại để tránh lộ thông tin tài khoản.
                return new ForgotPasswordResultDto
                {
                    Success = true,
                    Message = "Nếu email tồn tại trong hệ thống, mã đặt lại mật khẩu sẽ được tạo."
                };
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return new ForgotPasswordResultDto
            {
                Success = true,
                Message = "Tạo mã đặt lại mật khẩu thành công",
                ResetToken = resetToken
            };
        }

        //=================================ResetPassword========================================
        public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Mật khẩu xác nhận không khớp"
                };
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Email hoặc token không hợp lệ"
                };
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                dto.Token,
                dto.NewPassword
            );

            if (!result.Succeeded)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Đặt lại mật khẩu thất bại",
                    Errors = result.Errors
                        .Select(e => e.Description)
                        .ToList()
                };
            }
            await _userManager.UpdateSecurityStampAsync(user);

            return new AuthResultDto
            {
                Success = true,
                Message = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại."
            };
        }

        //=================================Logout========================================
        public async Task<AuthResultDto> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Không tìm thấy người dùng"
                };
            }

            // Đổi SecurityStamp
            // Tất cả JWT cũ chứa security_stamp cũ sẽ hết hiệu lực
            await _userManager.UpdateSecurityStampAsync(user);

            return new AuthResultDto
            {
                Success = true,
                Message = "Đăng xuất thành công. Token cũ đã hết hiệu lực."
            };
        }
    }
}