using Dapper;
using Microsoft.Extensions.Configuration;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;
using System.Data;

namespace SecurityBase.Infrastructure.Repositories;

public class RoleRepository : BaseRepository, IRoleRepository
{
    public RoleRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<int> CreateRoleAsync(Role role)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_CreateRole", 
            new { role.RoleName, role.Description }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<RoleDto>> GetRolesAsync()
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<RoleDto>("sp_GetRoles", 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> UpdateRoleAsync(Role role)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync("sp_UpdateRole", 
            new { role.RoleId, role.RoleName, role.Description }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> DeleteRoleAsync(int roleId)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync("sp_DeleteRole", 
            new { RoleId = roleId }, 
            commandType: CommandType.StoredProcedure);
    }
}
