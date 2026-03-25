using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<ApiResponse<int>> CreateRoleAsync(Role role)
    {
        try
        {
            var roleId = await _roleRepository.CreateRoleAsync(role);
            return new ApiResponse<int> { Success = true, Data = roleId, Message = "Role created successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<RoleDto>>> GetRolesAsync()
    {
        try
        {
            var roles = await _roleRepository.GetRolesAsync();
            return new ApiResponse<IEnumerable<RoleDto>> { Success = true, Data = roles };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<RoleDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> UpdateRoleAsync(Role role)
    {
        try
        {
            await _roleRepository.UpdateRoleAsync(role);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Role updated successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> DeleteRoleAsync(int roleId)
    {
        try
        {
            await _roleRepository.DeleteRoleAsync(roleId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Role deleted successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }
}
