using ASPtestShop.Auth;
using ASPtestShop.Data;
using ASPtestShop.Models.DTO.Order;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/orders")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + UserCookieAuth.Scheme)]
    public class OrderApiController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderNotificationService _orderNotificationService;

        public OrderApiController(IOrderService orderService, IOrderNotificationService orderNotificationService)
        {
            _orderService = orderService;
            _orderNotificationService = orderNotificationService;
        }

        //===============================CHECKOUT======================================
        [HttpPost("checkout")]
        [EnableRateLimiting("CheckoutRateLimit")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            //xác thực người dùng bản này để bắt chuỗi rỗng nếu chưa đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            // Tự động điền BaseUrl và IpAddress từ HTTP request (cổng thanh toán online cần)
            if (string.IsNullOrEmpty(checkoutDto.BaseUrl))
            {
                checkoutDto.BaseUrl = $"{Request.Scheme}://{Request.Host}";
            }
            if (string.IsNullOrEmpty(checkoutDto.IpAddress))
            {
                checkoutDto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            }

            var result = await _orderService.CheckoutAsync(userId, checkoutDto);
            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            // GIAI ĐOẠN 1: BẮN THÔNG BÁO REAL-TIME CHO ADMIN QUA SIGNALR
            _ = _orderNotificationService.NotifyAdminNewOrderAsync(
                result.OrderCode ?? "",
                checkoutDto.ReceiverName ?? "Khách hàng",
                result.FinalAmount,
                checkoutDto.PaymentMethod ?? "COD",
                result.PaymentStatus ?? "Pending"
            );

            return Ok(result);
        }

        //===============================GET ORDER HISTORY======================================
        //Lịch sử đơn hàng
        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var orders = await _orderService.GetOrderHistoryAsync(userId);

            if (!orders.Any())
            {
                return Ok(new
                {
                    Message = "Bạn chưa có đơn hàng nào",
                    Orders = orders
                });
            }

            return Ok(new
            {
                Message = "Lấy lịch sử đơn hàng thành công",
                Orders = orders
            });
        }

        //===============================GET ORDER DETAILS======================================
        // Lấy chi tiết lịch sử đơn hàng
        [Authorize]
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }
            // Lấy chi tiết đơn hàng dựa trên orderId và userId
            var order = await _orderService.GetOrderDetailAsync(userId, orderId);
            if (order == null)
            {
                return NotFound(new
                {
                    Message = "Đơn hàng không tồn tại hoặc bạn không có quyền truy cập"
                });
            }
            return Ok(new
            {
                Message = "Lấy chi tiết đơn hàng thành công",
                OrderDetails = order
            });
        }

        //===============================CANCEL ORDER======================================
        [HttpPost("cancel/{orderId:int}")]
        public async Task<IActionResult> CancelOrder(int orderId, [FromQuery] string? reason = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var result = await _orderService.CancelOrderAsync(userId, orderId, reason);
            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(result);
        }

        //===============================UC-19: AUTO CANCEL EXPIRED ORDERS================
        [HttpPost("auto-cancel-expired")]
        [AllowAnonymous] // Cho phép SystemScheduler hoặc Background worker gọi
        public async Task<IActionResult> AutoCancelExpiredOrders(
            [FromQuery] int timeoutHours = 24,
            [FromQuery] int onlineTimeoutMinutes = 30)
        {
            var result = await _orderService.AutoCancelExpiredOrdersAsync(timeoutHours, onlineTimeoutMinutes);
            return Ok(result);
        }
    }
}