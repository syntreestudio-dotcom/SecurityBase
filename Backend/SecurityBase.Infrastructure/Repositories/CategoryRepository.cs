using Dapper;
using Microsoft.Extensions.Configuration;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;
using System.Data;

namespace SecurityBase.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository, ICategoryRepository
{
    public CategoryRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<int> CreateCategoryAsync(Category category)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "sp_CreateCategory",
            new
            {
                category.CategoryTypeId,
                category.CategoryName,
                category.Code,
                category.Description,
                category.IsActive,
                category.DisplayOrder
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<CategoryDto>(
            "sp_GetCategories",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<DropdownItemDto>> GetCategoryOptionsByTypeAsync(string categoryTypeName)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<DropdownItemDto>(
            "sp_GetCategoryOptionsByTypeName",
            new { CategoryTypeName = categoryTypeName },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> UpdateCategoryAsync(Category category)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync(
            "sp_UpdateCategory",
            new
            {
                category.CategoryId,
                category.CategoryTypeId,
                category.CategoryName,
                category.Code,
                category.Description,
                category.IsActive,
                category.DisplayOrder
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> DeleteCategoryAsync(int categoryId)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync(
            "sp_DeleteCategory",
            new { CategoryId = categoryId },
            commandType: CommandType.StoredProcedure);
    }
}
