using System.Security.Claims;
using ASPtestShop.Auth;
using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Order;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/coupons")]
    public class CouponApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CouponApiController(AppDbContext context)
        {
            _context = context;
        }

        // POST: /api/coupons/apply
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.CouponCode))
            {
                return BadRequest(new ApplyCouponResultDto
                {
                    Success = false,
                    Message = "Vui lòng nhập mã giảm giá!"
                });
            }

            var code = request.CouponCode.Trim().ToUpperInvariant();
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);

            if (coupon == null || !coupon.IsActive)
            {
                return BadRequest(new ApplyCouponResultDto
                {
                    Success = false,
                    Message = "Mã giảm giá không tồn tại hoặc đã bị vô hiệu hóa!"
                });
            }

            var now = DateTime.UtcNow;
            if (now < coupon.StartDate || now > coupon.EndDate)
            {
                return BadRequest(new ApplyCouponResultDto
                {
                    Success = false,
                    Message = "Mã giảm giá chưa đến ngày bắt đầu hoặc đã hết hạn sử dụng!"
                });
            }

            if (coupon.UsedCount >= coupon.UsageLimitTotal)
            {
                return BadRequest(new ApplyCouponResultDto
                {
                    Success = false,
                    Message = "Mã giảm giá đã hết lượt sử dụng!"
                });
            }

            // Kiểm tra số lần sử dụng của khách hàng nếu đã đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var userUsedCount = await _context.Orders
                    .CountAsync(o => o.UserId == userId && o.CouponId == coupon.CouponId && o.OrderStatus != "Cancelled");

                if (userUsedCount >= coupon.UsageLimitPerCustomer)
                {
                    return BadRequest(new ApplyCouponResultDto
                    {
                        Success = false,
                        Message = $"Bạn đã sử dụng hết lượt ({coupon.UsageLimitPerCustomer} lần) cho mã giảm giá này!"
                    });
                }
            }

            var subtotal = request.Subtotal;
            // Nếu có CartItemIds, có thể tính toán lại chính xác từ DB
            if (request.CartItemIds != null && request.CartItemIds.Count > 0)
            {
                var dbTotal = await _context.CartItems
                    .Where(ci => request.CartItemIds.Contains(ci.CartItemId))
                    .Include(ci => ci.Product)
                    .SumAsync(ci => ci.Quantity * (ci.Product.SalePrice ?? ci.Product.Price));

                if (dbTotal > 0)
                {
                    subtotal = dbTotal;
                }
            }

            if (subtotal < coupon.MinOrderAmount)
            {
                return BadRequest(new ApplyCouponResultDto
                {
                    Success = false,
                    Message = $"Đơn hàng tối thiểu phải từ {coupon.MinOrderAmount:N0}đ để sử dụng mã này!"
                });
            }

            // Tính số tiền giảm giá
            decimal discountAmount = 0m;
            if (string.Equals(coupon.DiscountType, "Percentage", StringComparison.OrdinalIgnoreCase)
                || string.Equals(coupon.DiscountType, "Percent", StringComparison.OrdinalIgnoreCase))
            {
                discountAmount = subtotal * (coupon.DiscountValue / 100m);
                if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
                {
                    discountAmount = coupon.MaxDiscountAmount.Value;
                }
            }
            else
            {
                discountAmount = coupon.DiscountValue;
            }

            if (discountAmount > subtotal)
            {
                discountAmount = subtotal;
            }

            var finalAmount = Math.Max(subtotal - discountAmount, 0);

            return Ok(new ApplyCouponResultDto
            {
                Success = true,
                Message = $"Áp dụng mã '{coupon.Code}' thành công!",
                CouponCode = coupon.Code,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                DiscountAmount = discountAmount,
                Subtotal = subtotal,
                FinalAmount = finalAmount,
                MinOrderAmount = coupon.MinOrderAmount
            });
        }
    }
}
