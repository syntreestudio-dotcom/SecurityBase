using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<int>> CreateCategoryAsync(Category category)
    {
        try
        {
            var id = await _categoryRepository.CreateCategoryAsync(category);
            return new ApiResponse<int> { Success = true, Data = id, Message = "Category created successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoriesAsync()
    {
        try
        {
            var items = await _categoryRepository.GetCategoriesAsync();
            return new ApiResponse<IEnumerable<CategoryDto>> { Success = true, Data = items };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<CategoryDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<DropdownItemDto>>> GetCategoryOptionsByTypeAsync(string categoryTypeName)
    {
        try
        {
            var items = await _categoryRepository.GetCategoryOptionsByTypeAsync(categoryTypeName);
            return new ApiResponse<IEnumerable<DropdownItemDto>> { Success = true, Data = items };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<DropdownItemDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> UpdateCategoryAsync(Category category)
    {
        try
        {
            await _categoryRepository.UpdateCategoryAsync(category);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Category updated successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(int categoryId)
    {
        try
        {
            await _categoryRepository.DeleteCategoryAsync(categoryId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Category deleted successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }
}
