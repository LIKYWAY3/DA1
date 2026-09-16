using ASPtestShop.Data;
using ASPtestShop.Data.Entities;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Services.Implementations.Admin
{
    public class AdminCouponService : IAdminCouponService
    {
        private readonly AppDbContext _context;

        public AdminCouponService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminCouponDto>> GetCouponsAsync()
        {
            return await _context.Coupons.AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new AdminCouponDto
                {
                    CouponId = c.CouponId,
                    Code = c.Code,
                    DiscountType = c.DiscountType,
                    DiscountValue = c.DiscountValue,
                    MinOrderAmount = c.MinOrderAmount,
                    MaxDiscountAmount = c.MaxDiscountAmount,
                    UsageLimitTotal = c.UsageLimitTotal,
                    UsageLimitPerCustomer = c.UsageLimitPerCustomer,
                    UsedCount = c.UsedCount,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<AdminCouponDto?> GetCouponByIdAsync(int id)
        {
            var c = await _context.Coupons.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CouponId == id);

            if (c == null) return null;

            return new AdminCouponDto
            {
                CouponId = c.CouponId,
                Code = c.Code,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                MinOrderAmount = c.MinOrderAmount,
                MaxDiscountAmount = c.MaxDiscountAmount,
                UsageLimitTotal = c.UsageLimitTotal,
                UsageLimitPerCustomer = c.UsageLimitPerCustomer,
                UsedCount = c.UsedCount,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            };
        }

        public async Task<(bool Success, string Message, int? CouponId)> CreateCouponAsync(CreateCouponDto dto)
        {
            var codeUpper = dto.Code.Trim().ToUpperInvariant();

            // Kiểm tra trùng Code
            var exists = await _context.Coupons.AnyAsync(c => c.Code == codeUpper);
            if (exists)
            {
                return (false, $"Mã giảm giá '{codeUpper}' đã tồn tại trên hệ thống!", null);
            }

            if (dto.EndDate <= dto.StartDate)
            {
                return (false, "Ngày kết thúc phải sau ngày bắt đầu hiệu lực!", null);
            }

            var coupon = new Coupon
            {
                Code = codeUpper,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinOrderAmount = dto.MinOrderAmount,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                UsageLimitTotal = dto.UsageLimitTotal,
                UsageLimitPerCustomer = dto.UsageLimitPerCustomer,
                UsedCount = 0,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return (true, "Tạo mã giảm giá thành công!", coupon.CouponId);
        }

        public async Task<(bool Success, string Message)> UpdateCouponAsync(int id, UpdateCouponDto dto)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return (false, "Không tìm thấy mã giảm giá cần cập nhật!");
            }

            if (dto.EndDate <= dto.StartDate)
            {
                return (false, "Ngày kết thúc phải sau ngày bắt đầu hiệu lực!");
            }

            coupon.DiscountType = dto.DiscountType;
            coupon.DiscountValue = dto.DiscountValue;
            coupon.MinOrderAmount = dto.MinOrderAmount;
            coupon.MaxDiscountAmount = dto.MaxDiscountAmount;
            coupon.UsageLimitTotal = dto.UsageLimitTotal;
            coupon.UsageLimitPerCustomer = dto.UsageLimitPerCustomer;
            coupon.StartDate = dto.StartDate;
            coupon.EndDate = dto.EndDate;
            coupon.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return (true, "Cập nhật mã giảm giá thành công!");
        }

        public async Task<(bool Success, string Message, bool NewStatus)> ToggleActiveAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return (false, "Không tìm thấy mã giảm giá!", false);
            }

            coupon.IsActive = !coupon.IsActive;
            await _context.SaveChangesAsync();

            var statusText = coupon.IsActive ? "kích hoạt" : "vô hiệu hóa";
            return (true, $"Đã {statusText} mã giảm giá '{coupon.Code}'!", coupon.IsActive);
        }

        public async Task<(bool Success, string Message)> DeleteCouponAsync(int id)
        {
            var coupon = await _context.Coupons
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CouponId == id);

            if (coupon == null)
            {
                return (false, "Không tìm thấy mã giảm giá cần xóa!");
            }

            // Nếu mã đã được áp dụng trong đơn hàng nào đó, chuyển sang soft delete IsActive = false
            if (coupon.Orders.Any())
            {
                coupon.IsActive = false;
                await _context.SaveChangesAsync();
                return (true, "Mã đã có đơn hàng sử dụng nên hệ thống đã chuyển sang trạng thái Vô hiệu hóa (Soft Delete)!");
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return (true, "Đã xóa mã giảm giá thành công!");
        }
    }
}
