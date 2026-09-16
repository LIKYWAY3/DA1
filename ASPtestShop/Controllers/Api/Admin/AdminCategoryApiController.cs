using ASPtestShop.Auth;
using ASPtestShop.Models.DTO.Category;
using ASPtestShop.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPtestShop.Controllers.Api.Admin
{
    [Route("api/admin/categories")]
    [ApiController]
    [Authorize(
    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + AdminCookieAuth.Scheme, Roles = "Admin")]
    public class AdminCategoryApiController : ControllerBase
    {
        private readonly IAdminCategoryService _adminCategoryService;

        public AdminCategoryApiController(IAdminCategoryService adminCategoryService)
        {
            _adminCategoryService = adminCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _adminCategoryService.GetCategoriesAsync();

            return Ok(categories);
        }

        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var category = await _adminCategoryService.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                return NotFound(new
                {
                    Message = "Không tìm thấy danh mục"
                });
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
        {
            var result = await _adminCategoryService.CreateCategoryAsync(dto);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(new
            {
                result.Message,
                result.Category
            });
        }

        [HttpPut("{categoryId:int}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, UpdateCategoryDto dto)
        {
            var result = await _adminCategoryService.UpdateCategoryAsync(categoryId, dto);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(new
            {
                result.Message,
                result.Category
            });
        }

        [HttpDelete("{categoryId:int}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var result = await _adminCategoryService.DeleteCategoryAsync(categoryId);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(new
            {
                result.Message,
                result.Category
            });
        }

        [HttpPost("migrate-taxonomy")]
        public async Task<IActionResult> MigrateTaxonomy()
        {
            var result = await _adminCategoryService.MigrateTaxonomyAsync();

            if (!result.Success)
            {
                return BadRequest(new
                {
                    result.Message
                });
            }

            return Ok(new
            {
                result.Message
            });
        }
    }
}