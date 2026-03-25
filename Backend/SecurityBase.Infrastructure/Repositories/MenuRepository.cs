using Dapper;
using Microsoft.Extensions.Configuration;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;
using System.Data;

namespace SecurityBase.Infrastructure.Repositories;

public class MenuRepository : BaseRepository, IMenuRepository
{
    public MenuRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<int> CreateMenuAsync(Menu menu)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_CreateMenu", 
            new { menu.MenuName, menu.ParentMenuId, menu.Route, menu.Icon, menu.DisplayOrder }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Menu>> GetMenusAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Menu>("sp_GetMenus", 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> UpdateMenuAsync(Menu menu)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync("sp_UpdateMenu", 
            new { menu.MenuId, menu.MenuName, menu.ParentMenuId, menu.Route, menu.Icon, menu.DisplayOrder }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> DeleteMenuAsync(int menuId)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync("sp_DeleteMenu", 
            new { MenuId = menuId }, 
            commandType: CommandType.StoredProcedure);
    }
}
