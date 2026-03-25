using Dapper;
using Microsoft.Extensions.Configuration;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;
using System.Data;

namespace SecurityBase.Infrastructure.Repositories;

public class AssignmentRepository : BaseRepository, IAssignmentRepository
{
    public AssignmentRepository(IConfiguration configuration) : base(configuration) { }

    public async Task AssignRoleToUserAsync(int userId, int roleId)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync("sp_AssignRoleToUser", 
            new { UserId = userId, RoleId = roleId }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task RevokeRoleFromUserAsync(int userId, int roleId)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync("sp_RevokeRoleFromUser",
            new { UserId = userId, RoleId = roleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task SetUserSingleRoleAsync(int userId, int roleId)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync("sp_SetUserSingleRole",
            new { UserId = userId, RoleId = roleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task AssignMenuToRoleAsync(int roleId, int menuId)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync("sp_AssignMenuToRole",
            new { RoleId = roleId, MenuId = menuId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task RevokeMenuFromRoleAsync(int roleId, int menuId)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync("sp_RevokeMenuFromRole",
            new { RoleId = roleId, MenuId = menuId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Role>("sp_GetUserRoles", 
            new { UserId = userId }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Menu>> GetUserMenusAsync(int userId)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Menu>("sp_GetUserMenus", 
            new { UserId = userId }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Menu>> GetRoleMenusAsync(int roleId)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<Menu>("sp_GetRoleMenus",
            new { RoleId = roleId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<(User? user, IEnumerable<string> roles, IEnumerable<Menu> menus)> GetUserPermissionsAsync(string username)
    {
        using var connection = CreateConnection();
        using var multi = await connection.QueryMultipleAsync("sp_GetUserPermissions", 
            new { Username = username }, 
            commandType: CommandType.StoredProcedure);

        var user = await multi.ReadFirstOrDefaultAsync<User>();
        var roles = await multi.ReadAsync<string>();
        var menus = await multi.ReadAsync<Menu>();

        return (user, roles, menus);
    }
}
