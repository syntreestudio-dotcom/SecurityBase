using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Infrastructure.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;

    public AssignmentService(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<ApiResponse<bool>> AssignRoleToUserAsync(int userId, int roleId)
    {
        try
        {
            await _assignmentRepository.AssignRoleToUserAsync(userId, roleId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Role assigned successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> RevokeRoleFromUserAsync(int userId, int roleId)
    {
        try
        {
            await _assignmentRepository.RevokeRoleFromUserAsync(userId, roleId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Role revoked successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> SetUserSingleRoleAsync(int userId, int roleId)
    {
        try
        {
            await _assignmentRepository.SetUserSingleRoleAsync(userId, roleId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "User role updated successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> AssignMenuToRoleAsync(int roleId, int menuId)
    {
        try
        {
            await _assignmentRepository.AssignMenuToRoleAsync(roleId, menuId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Menu assigned successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> RevokeMenuFromRoleAsync(int roleId, int menuId)
    {
        try
        {
            await _assignmentRepository.RevokeMenuFromRoleAsync(roleId, menuId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Menu revoked successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<Role>>> GetUserRolesAsync(int userId)
    {
        try
        {
            var roles = await _assignmentRepository.GetUserRolesAsync(userId);
            return new ApiResponse<IEnumerable<Role>> { Success = true, Data = roles };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<Role>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<Menu>>> GetUserMenusAsync(int userId)
    {
        try
        {
            var menus = await _assignmentRepository.GetUserMenusAsync(userId);
            return new ApiResponse<IEnumerable<Menu>> { Success = true, Data = menus };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<Menu>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<IEnumerable<Menu>>> GetRoleMenusAsync(int roleId)
    {
        try
        {
            var menus = await _assignmentRepository.GetRoleMenusAsync(roleId);
            return new ApiResponse<IEnumerable<Menu>> { Success = true, Data = menus };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<Menu>> { Success = false, Message = ex.Message };
        }
    }
}
