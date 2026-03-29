using Dapper;
using Microsoft.Extensions.Configuration;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;
using System.Data;

namespace SecurityBase.Infrastructure.Repositories;

public class CategoryTypeRepository : BaseRepository, ICategoryTypeRepository
{
    public CategoryTypeRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<int> CreateCategoryTypeAsync(CategoryType categoryType)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            "sp_CreateCategoryType",
            new
            {
                categoryType.CategoryTypeName,
                categoryType.Description,
                categoryType.IsActive,
                categoryType.DisplayOrder
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<CategoryType>> GetCategoryTypesAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<CategoryType>(
            "sp_GetCategoryTypes",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> UpdateCategoryTypeAsync(CategoryType categoryType)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync(
            "sp_UpdateCategoryType",
            new
            {
                categoryType.CategoryTypeId,
                categoryType.CategoryTypeName,
                categoryType.Description,
                categoryType.IsActive,
                categoryType.DisplayOrder
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> DeleteCategoryTypeAsync(int categoryTypeId)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync(
            "sp_DeleteCategoryType",
            new { CategoryTypeId = categoryTypeId },
            commandType: CommandType.StoredProcedure);
    }
}
