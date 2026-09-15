using System.Security.Claims;
using ASPtestShop.Models.DTO.Cart;
using ASPtestShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme + "," + ASPtestShop.Auth.UserCookieAuth.Scheme)]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartApiController(ICartService cartService)
        {
            _cartService = cartService;
        }

        //====================================ADD TO CART======================================
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var result = await _cartService.AddToCartAsync(userId, dto);

            return HandleCartResult(result);
        }

        //====================================GET CART======================================
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var result = await _cartService.GetCartAsync(userId);

            return Ok(result);
        }

        //====================================UPDATE CART ITEM======================================
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var result = await _cartService.UpdateCartItemAsync(userId, dto);

            return HandleCartResult(result);
        }

        //====================================REMOVE CART ITEM======================================
        [HttpDelete("remove/{cartItemId:int}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var result = await _cartService.RemoveCartItemAsync(userId, cartItemId);

            return HandleCartResult(result);
        }

        //====================================CLEAR CART======================================
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "Bạn chưa đăng nhập"
                });
            }

            var result = await _cartService.ClearCartAsync(userId);

            return Ok(result);
        }

        private IActionResult HandleCartResult(CartResultDto result)
        {
            if (result.Success)
            {
                return Ok(result);
            }

            if (result.ErrorCode == "NotFound")
            {
                return NotFound(new
                {
                    result.Message
                });
            }

            return BadRequest(new
            {
                result.Message
            });
        }
    }
}