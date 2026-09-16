using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AdminUserService(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<AdminUserDto>> GetUsersAsync(string? search = null)
        {
            var query = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(u =>
                    (u.Email != null && u.Email.ToLower().Contains(s)) ||
                    (u.UserName != null && u.UserName.ToLower().Contains(s)) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(s)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(s))
                );
            }

            var users = await query.Take(100).ToListAsync();
            var userIds = users.Select(u => u.Id).ToList();

            // Tính thống kê đơn hàng và tổng tiền mua của mỗi user
            var orderStats = await _context.Orders.AsNoTracking()
                .Where(o => o.UserId != null && userIds.Contains(o.UserId))
                .GroupBy(o => o.UserId!)
                .Select(g => new
                {
                    UserId = g.Key,
                    OrderCount = g.Count(),
                    TotalSpent = g.Where(o => o.PaymentStatus == "Paid" || o.OrderStatus == "Delivered" || o.OrderStatus == "Completed")
                                  .Sum(o => (decimal?)o.FinalAmount) ?? 0m
                })
                .ToDictionaryAsync(x => x.UserId, x => new { x.OrderCount, x.TotalSpent });

            var now = DateTimeOffset.UtcNow;
            var result = new List<AdminUserDto>();

            foreach (var u in users)
            {
                var isLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value > now;
                orderStats.TryGetValue(u.Id, out var stat);

                result.Add(new AdminUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName ?? "",
                    Email = u.Email ?? "",
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address,
                    OrderCount = stat?.OrderCount ?? 0,
                    TotalSpent = stat?.TotalSpent ?? 0m,
                    IsLockedOut = isLocked,
                    LockoutEnd = u.LockoutEnd
                });
            }

            return result;
        }

        public async Task<(bool Success, string Message, bool IsLockedOut)> ToggleLockUserAsync(string currentAdminId, string targetUserId)
        {
            if (currentAdminId == targetUserId)
            {
                return (false, "Không thể tự khóa tài khoản quản trị viên đang đăng nhập!", false);
            }

            var user = await _userManager.FindByIdAsync(targetUserId);
            if (user == null)
            {
                return (false, "Không tìm thấy người dùng cần thao tác!", false);
            }

            var now = DateTimeOffset.UtcNow;
            var currentlyLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > now;

            if (currentlyLocked)
            {
                // Mở khóa tài khoản
                await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.ResetAccessFailedCountAsync(user);
                return (true, $"Đã mở khóa tài khoản '{user.UserName}' thành công!", false);
            }
            else
            {
                // Khóa tài khoản (BR-34: 100 năm)
                user.LockoutEnabled = true;
                await _userManager.SetLockoutEndDateAsync(user, now.AddYears(100));
                await _userManager.UpdateSecurityStampAsync(user); // Vô hiệu hóa phiên đăng nhập cũ
                return (true, $"Đã khóa tài khoản '{user.UserName}' thành công!", true);
            }
        }
    }
}
