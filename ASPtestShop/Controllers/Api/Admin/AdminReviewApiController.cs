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

        // POST: /api/admin/reviews/batch-approve
        [HttpPost("batch-approve")]
        public async Task<IActionResult> BatchApprove([FromBody] ASPtestShop.Models.DTO.Admin.BatchReviewRequestDto dto)
        {
            if (dto?.ReviewIds == null || !dto.ReviewIds.Any())
            {
                return BadRequest(new ASPtestShop.Models.DTO.Admin.BatchReviewActionResultDto
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một đánh giá để duyệt."
                });
            }

            var result = await _reviewService.BatchApproveReviewsAsync(dto.ReviewIds);
            return Ok(result);
        }

        // POST: /api/admin/reviews/batch-reject
        [HttpPost("batch-reject")]
        public async Task<IActionResult> BatchReject([FromBody] ASPtestShop.Models.DTO.Admin.BatchReviewRequestDto dto)
        {
            if (dto?.ReviewIds == null || !dto.ReviewIds.Any())
            {
                return BadRequest(new ASPtestShop.Models.DTO.Admin.BatchReviewActionResultDto
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một đánh giá để ẩn."
                });
            }

            var result = await _reviewService.BatchRejectReviewsAsync(dto.ReviewIds);
            return Ok(result);
        }

        // POST: /api/admin/reviews/batch-delete
        [HttpPost("batch-delete")]
        public async Task<IActionResult> BatchDelete([FromBody] ASPtestShop.Models.DTO.Admin.BatchReviewRequestDto dto)
        {
            if (dto?.ReviewIds == null || !dto.ReviewIds.Any())
            {
                return BadRequest(new ASPtestShop.Models.DTO.Admin.BatchReviewActionResultDto
                {
                    Success = false,
                    Message = "Vui lòng chọn ít nhất một đánh giá để xóa."
                });
            }

            var result = await _reviewService.BatchDeleteReviewsAsync(dto.ReviewIds);
            return Ok(result);
        }
    }
}
