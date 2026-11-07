using System.Security.Claims;
using ASPtestShop.Models.DTO.Refund;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    /// <summary>
    /// UC-20: API Yêu cầu hoàn tiền (RefundApiController)
    /// Hỗ trợ cả xác thực Cookie lẫn JWT Token.
    /// </summary>
    [ApiController]
    [Route("api/refunds")]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme + "," + ASPtestShop.Auth.UserCookieAuth.Scheme + "," + ASPtestShop.Auth.AdminCookieAuth.Scheme)]
    public class RefundApiController : ControllerBase
    {
        private readonly IRefundService _refundService;

        public RefundApiController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        // =======================================================================
        // POST: api/refunds
        // Khách hàng gửi yêu cầu hoàn tiền
        // =======================================================================
        [HttpPost]
        public async Task<IActionResult> RequestRefund([FromBody] CreateRefundRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Bạn chưa đăng nhập" });
            }

            var result = await _refundService.RequestRefundAsync(userId, dto);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(result);
        }

        // =======================================================================
        // GET: api/refunds/my
        // Khách hàng xem danh sách yêu cầu hoàn tiền của mình
        // =======================================================================
        [HttpGet("my")]
        public async Task<IActionResult> GetMyRefunds()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Bạn chưa đăng nhập" });
            }

            var requests = await _refundService.GetMyRefundRequestsAsync(userId);
            return Ok(requests);
        }

        // =======================================================================
        // GET: api/refunds/order/{orderId}
        // Xem yêu cầu hoàn tiền của một đơn hàng
        // =======================================================================
        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetRefundByOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var request = await _refundService.GetRefundRequestByOrderIdAsync(orderId, isAdmin ? "" : (userId ?? ""));
            if (request == null)
            {
                return NotFound(new { Message = "Không có yêu cầu hoàn tiền cho đơn hàng này." });
            }

            return Ok(request);
        }

        // =======================================================================
        // GET: api/refunds/admin/all
        // Admin: Xem tất cả yêu cầu hoàn tiền
        // =======================================================================
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRefundsForAdmin()
        {
            var requests = await _refundService.GetAllRefundRequestsAsync();
            return Ok(requests);
        }

        // =======================================================================
        // GET: api/refunds/admin/pending-count
        // Admin: Đếm số lượng yêu cầu hoàn tiền chờ duyệt
        // =======================================================================
        [HttpGet("admin/pending-count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingRefundCount()
        {
            var count = await _refundService.GetPendingRefundCountAsync();
            return Ok(new { count });
        }

        // =======================================================================
        // PUT: api/refunds/admin/{id}/process
        // Admin: Phê duyệt hoặc từ chối hoàn tiền
        // =======================================================================
        [HttpPut("admin/{id:int}/process")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessRefund(int id, [FromBody] ProcessRefundDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Admin";
            var result = await _refundService.ProcessRefundAsync(id, adminUserId, dto);

            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(result);
        }

        // =======================================================================
        // POST: api/refunds/resubmit
        // Khách hàng gửi lại thông tin tài khoản khi yêu cầu bị từ chối
        // =======================================================================
        [HttpPost("resubmit")]
        public async Task<IActionResult> ResubmitRefund([FromBody] ResubmitRefundRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Bạn chưa đăng nhập" });
            }

            var result = await _refundService.ResubmitRefundRequestAsync(userId, dto);
            if (!result.Success)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(result);
        }
    }
}
