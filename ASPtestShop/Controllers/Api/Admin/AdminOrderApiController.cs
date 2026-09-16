using ASPtestShop.Auth;
using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + AdminCookieAuth.Scheme,
        Roles = "Admin"
    )]
    public class AdminOrderApiController : ControllerBase
    {
        private readonly IAdminOrderService _adminOrderService;

        public AdminOrderApiController(IAdminOrderService adminOrderService)
        {
            _adminOrderService = adminOrderService;
        }

        // GET: /api/admin/orders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _adminOrderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        // GET: /api/admin/orders/5
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _adminOrderService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy đơn hàng"
                });
            }

            return Ok(order);
        }

        // PUT: /api/admin/orders/5/status
        [HttpPut("{orderId:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ"
                });
            }

            var result = await _adminOrderService.UpdateOrderStatusAsync(orderId, dto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // DELETE: /api/admin/orders/5
        [HttpDelete("{orderId:int}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var result = await _adminOrderService.DeleteCancelledOrderAsync(orderId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // DELETE: /api/admin/orders/cleanup-cancelled
        [HttpDelete("cleanup-cancelled")]
        public async Task<IActionResult> CleanupCancelledOrders()
        {
            var result = await _adminOrderService.CleanupAllCancelledOrdersAsync();
            return Ok(result);
        }
    }
}