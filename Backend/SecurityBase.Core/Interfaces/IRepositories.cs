using SecurityBase.Core.Entities;
using SecurityBase.Core.DTOs;

namespace SecurityBase.Core.Interfaces;

public interface IUserRepository
{
    Task<int> CreateUserAsync(User user);
    Task<IEnumerable<UserDto>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm);
    Task<int> UpdateUserAsync(User user);
    Task<int> UpdateUserPasswordAsync(int userId, string passwordHash);
    Task<int> DeleteUserAsync(int userId);
    Task<User?> GetUserByUsernameAsync(string username);
}

public interface IRoleRepository
{
    Task<int> CreateRoleAsync(Role role);
    Task<IEnumerable<RoleDto>> GetRolesAsync();
    Task<int> UpdateRoleAsync(Role role);
    Task<int> DeleteRoleAsync(int roleId);
}

public interface IMenuRepository
{
    Task<int> CreateMenuAsync(Menu menu);
    Task<IEnumerable<Menu>> GetMenusAsync();
    Task<int> UpdateMenuAsync(Menu menu);
    Task<int> DeleteMenuAsync(int menuId);
}

public interface IAssignmentRepository
{
    Task AssignRoleToUserAsync(int userId, int roleId);
    Task RevokeRoleFromUserAsync(int userId, int roleId);
    Task SetUserSingleRoleAsync(int userId, int roleId);
    Task AssignMenuToRoleAsync(int roleId, int menuId);
    Task RevokeMenuFromRoleAsync(int roleId, int menuId);
    Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
    Task<IEnumerable<Menu>> GetUserMenusAsync(int userId);
    Task<IEnumerable<Menu>> GetRoleMenusAsync(int roleId);
    Task<(User? user, IEnumerable<string> roles, IEnumerable<Menu> menus)> GetUserPermissionsAsync(string username);
}
