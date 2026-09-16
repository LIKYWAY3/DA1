using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Auth;
using ASPtestShop.Models.ViewModels.Auth;
using ASPtestShop.Models.ViewModels.Profile;

namespace ASPtestShop.Services.Interfaces.User
{
    public interface IUserAuthService
    {
        Task<UserLoginResultDto> LoginAsync(LoginViewModel model);

        Task<UserRegisterResultDto> RegisterAsync(RegisterViewModel model);

        Task<UserRegisterResultDto> InitiateRegisterAsync(RegisterViewModel model);

        Task<UserRegisterResultDto> VerifyOtpAndRegisterAsync(VerifyEmailViewModel model);

        Task<UserRegisterResultDto> ResendOtpAsync(string email);

        Task<AuthResultDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);

        Task<UserProfileDto?> GetUserProfileAsync(string userId);

        Task<List<BankAccountDto>> GetBankAccountsAsync(string userId);

        Task<AuthResultDto> AddBankAccountAsync(string userId, AddBankAccountViewModel model);

        Task<List<UserAddress>> GetUserAddressesAsync(string userId);

        Task<AuthResultDto> AddAddressAsync(string userId, AddAddressViewModel model);

        Task<AuthResultDto> SetDefaultAddressAsync(string userId, int addressId);

        Task<AuthResultDto> EditAddressAsync(string userId, EditAddressViewModel model);

        Task<AuthResultDto> ChangePasswordAsync(string userId, ChangePasswordViewModel model);

        Task<ForgotPasswordResultDto> ForgotPasswordAsync(ForgotPasswordViewModel model);

        Task<ResetPasswordResultDto> ResetPasswordAsync(ResetPasswordViewModel model);
    }
}