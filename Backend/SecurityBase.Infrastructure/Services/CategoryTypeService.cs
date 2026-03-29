using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Infrastructure.Services;

public class CategoryTypeService : ICategoryTypeService
{
    private readonly ICategoryTypeRepository _categoryTypeRepository;

    public CategoryTypeService(ICategoryTypeRepository categoryTypeRepository)
    {
        _categoryTypeRepository = categoryTypeRepository;
    }

    public async Task<ApiResponse<int>> CreateCategoryTypeAsync(CategoryType categoryType)
    {
        try
        {
            var id = await _categoryTypeRepository.CreateCategoryTypeAsync(categoryType);
            return new ApiResponse<int> { Success = true, Data = id, Message = "Category type created successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<CategoryType>>> GetCategoryTypesAsync()
    {
        try
        {
            var items = await _categoryTypeRepository.GetCategoryTypesAsync();
            return new ApiResponse<IEnumerable<CategoryType>> { Success = true, Data = items };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<CategoryType>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<DropdownItemDto>>> GetCategoryTypeOptionsAsync()
    {
        try
        {
            var items = await _categoryTypeRepository.GetCategoryTypesAsync();
            var options = items.Select(i => new DropdownItemDto
            {
                Id = i.CategoryTypeId,
                Name = i.CategoryTypeName
            });
            return new ApiResponse<IEnumerable<DropdownItemDto>> { Success = true, Data = options };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<DropdownItemDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> UpdateCategoryTypeAsync(CategoryType categoryType)
    {
        try
        {
            await _categoryTypeRepository.UpdateCategoryTypeAsync(categoryType);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Category type updated successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> DeleteCategoryTypeAsync(int categoryTypeId)
    {
        try
        {
            await _categoryTypeRepository.DeleteCategoryTypeAsync(categoryTypeId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Category type deleted successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }
}
