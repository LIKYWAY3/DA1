using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ASPtestShop.Models.DTO.Auth;

namespace ASPtestShop.Models.ViewModels.Profile
{
    public class UserProfileViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [Display(Name = "Tên đăng nhập (Không thể đổi)")]
        public string? UserName { get; set; }

        [Display(Name = "Địa chỉ Email")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Định dạng Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? AvatarUrl { get; set; }

        [Display(Name = "Tải ảnh đại diện mới")]
        public IFormFile? AvatarFile { get; set; }

        public string MaskedEmail
        {
            get
            {
                if (string.IsNullOrEmpty(Email) || !Email.Contains("@"))
                    return "Chưa cập nhật";

                var parts = Email.Split('@');
                string name = parts[0];
                string domain = parts[1];

                if (name.Length <= 1) return Email;

                return name.Substring(0, 1) + new string('•', 8) + "@" + domain;
            }
        }

        public string MaskedPhoneNumber
        {
            get
            {
                if (string.IsNullOrEmpty(PhoneNumber) || PhoneNumber.Length <= 2)
                    return "Chưa cập nhật";

                return new string('•', PhoneNumber.Length - 2) + PhoneNumber.Substring(PhoneNumber.Length - 2);
            }
        }
        public List<BankAccountDto>? BankAccounts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public List<ASPtestShop.Data.Entities.UserAddress>? Addresses { get; set; }
        public ASPtestShop.Data.Entities.UserAddress? DefaultAddress { get; set; }
        public bool IsGoogleLinked { get; set; }
    }
}