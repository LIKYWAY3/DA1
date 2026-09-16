using ASPtestShop.Auth;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/reviews")]
    [Authorize(AuthenticationSchemes = AdminCookieAuth.Scheme, Roles = "Admin")]
    public class AdminReviewApiController : ControllerBase
    {
        private readonly IAdminReviewService _reviewService;

        public AdminReviewApiController(IAdminReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: /api/admin/reviews?filter=pending|approved|all
        [HttpGet("")]
        public async Task<IActionResult> GetReviews([FromQuery] string? filter = "all")
        {
            var list = await _reviewService.GetReviewsAsync(filter);
            return Ok(list);
        }

        // POST: /api/admin/reviews/{id}/approve
        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _reviewService.ApproveReviewAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        // POST: /api/admin/reviews/{id}/reject
        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var result = await _reviewService.RejectReviewAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        // DELETE: /api/admin/reviews/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
