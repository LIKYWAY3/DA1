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

        public async Task<AdminUserDetailDto?> GetUserDetailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var now = DateTimeOffset.UtcNow;
            var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > now;

            // Lấy danh sách địa chỉ
            var addresses = await _context.UserAddresses.AsNoTracking()
                .Where(a => a.UserId == userId)
                .Select(a => new AdminUserAddressItemDto
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    PhoneNumber = a.PhoneNumber,
                    SpecificAddress = a.SpecificAddress,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();

            // Lấy danh sách đơn hàng
            var orders = await _context.Orders.AsNoTracking()
                .Include(o => o.OrderItems)
                .Include(o => o.Coupon)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Thống kê đơn hàng
            var totalOrders = orders.Count;
            var completedOrders = orders.Count(o => o.OrderStatus == "Completed" || o.OrderStatus == "Delivered");
            var cancelledOrders = orders.Count(o => o.OrderStatus == "Cancelled");
            var totalSpent = orders
                .Where(o => o.PaymentStatus == "Paid" || o.OrderStatus == "Delivered" || o.OrderStatus == "Completed")
                .Sum(o => o.FinalAmount);

            var totalReviews = await _context.Reviews.CountAsync(r => r.UserId == userId);

            return new AdminUserDetailDto
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                EmailConfirmed = user.EmailConfirmed,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Gender = user.Gender,
                AvatarUrl = user.AvatarUrl,
                PrimaryAddress = user.Address,
                IsLockedOut = isLocked,
                LockoutEnd = user.LockoutEnd,
                AccessFailedCount = user.AccessFailedCount,
                TotalOrders = totalOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders,
                TotalSpent = totalSpent,
                TotalReviews = totalReviews,
                Addresses = addresses,
                RecentOrders = orders.Take(20).Select(o => new AdminUserOrderSummaryDto
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    DiscountAmount = o.DiscountAmount,
                    FinalAmount = o.FinalAmount,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    ItemCount = o.OrderItems?.Count ?? 0,
                    CouponCode = o.Coupon?.Code
                }).ToList()
            };
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
