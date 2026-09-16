using ASPtestShop.Auth;
using ASPtestShop.Models.DTO.Admin;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/coupons")]
    [Authorize(AuthenticationSchemes = AdminCookieAuth.Scheme, Roles = "Admin")]
    public class AdminCouponApiController : ControllerBase
    {
        private readonly IAdminCouponService _couponService;

        public AdminCouponApiController(IAdminCouponService couponService)
        {
            _couponService = couponService;
        }

        // GET: /api/admin/coupons
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var coupons = await _couponService.GetCouponsAsync();
            return Ok(coupons);
        }

        // GET: /api/admin/coupons/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null) return NotFound(new { message = "Không tìm thấy mã giảm giá" });
            return Ok(coupon);
        }

        // POST: /api/admin/coupons
        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] CreateCouponDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _couponService.CreateCouponAsync(dto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message, couponId = result.CouponId });
        }

        // PUT: /api/admin/coupons/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _couponService.UpdateCouponAsync(id, dto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        // POST: /api/admin/coupons/{id}/toggle-active
        [HttpPost("{id:int}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var result = await _couponService.ToggleActiveAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message, newStatus = result.NewStatus });
        }

        // DELETE: /api/admin/coupons/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }
    }
}
