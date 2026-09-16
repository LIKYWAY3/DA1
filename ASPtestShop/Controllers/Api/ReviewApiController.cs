using System.Security.Claims;
using ASPtestShop.Auth;
using ASPtestShop.Models.DTO.Review;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewApiController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewApiController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: /api/reviews/product/5
        [HttpGet("product/{productId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var result = await _reviewService.GetProductReviewsAsync(productId);
            return Ok(result);
        }

        // POST: /api/reviews
        [HttpPost("")]
        [Authorize(AuthenticationSchemes = UserCookieAuth.Scheme + "," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "Bạn cần đăng nhập để viết đánh giá!" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu đánh giá không hợp lệ!" });
            }

            var (success, message, review) = await _reviewService.CreateReviewAsync(userId, dto);
            if (!success)
            {
                return BadRequest(new { success = false, message });
            }

            return Ok(new { success = true, message, data = review });
        }
    }
}
