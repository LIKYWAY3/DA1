using ASPtestShop.Models.DTO.Category;

namespace ASPtestShop.Services.Interfaces.Admin;

public interface IAdminCategoryService
{
    Task<List<AdminCategoryResultDto>> GetCategoriesAsync();

    Task<AdminCategoryResultDto?> GetCategoryByIdAsync(int categoryId);

    Task<AdminCategoryActionResultDto> CreateCategoryAsync(CreateCategoryDto dto);

    Task<AdminCategoryActionResultDto> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto);

    Task<AdminCategoryActionResultDto> DeleteCategoryAsync(int categoryId);

    Task<AdminCategoryActionResultDto> MigrateTaxonomyAsync();
}