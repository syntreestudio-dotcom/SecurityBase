using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;

namespace SecurityBase.Core.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
}

public interface IUserService
{
    Task<ApiResponse<int>> CreateUserAsync(User user);
    Task<ApiResponse<PaginatedList<UserDto>>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm);
    Task<ApiResponse<bool>> UpdateUserAsync(User user);
    Task<ApiResponse<bool>> UpdateUserPasswordAsync(int userId, string newPassword);
    Task<ApiResponse<bool>> DeleteUserAsync(int userId);
}

public interface IRoleService
{
    Task<ApiResponse<int>> CreateRoleAsync(Role role);
    Task<ApiResponse<IEnumerable<RoleDto>>> GetRolesAsync();
    Task<ApiResponse<bool>> UpdateRoleAsync(Role role);
    Task<ApiResponse<bool>> DeleteRoleAsync(int roleId);
}

public interface IMenuService
{
    Task<ApiResponse<int>> CreateMenuAsync(Menu menu);
    Task<ApiResponse<IEnumerable<Menu>>> GetMenusAsync();
    Task<ApiResponse<bool>> UpdateMenuAsync(Menu menu);
    Task<ApiResponse<bool>> DeleteMenuAsync(int menuId);
}

public interface ICategoryTypeService
{
    Task<ApiResponse<int>> CreateCategoryTypeAsync(CategoryType categoryType);
    Task<ApiResponse<IEnumerable<CategoryType>>> GetCategoryTypesAsync();
    Task<ApiResponse<IEnumerable<DropdownItemDto>>> GetCategoryTypeOptionsAsync();
    Task<ApiResponse<bool>> UpdateCategoryTypeAsync(CategoryType categoryType);
    Task<ApiResponse<bool>> DeleteCategoryTypeAsync(int categoryTypeId);
}

public interface ICategoryService
{
    Task<ApiResponse<int>> CreateCategoryAsync(Category category);
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoriesAsync();
    Task<ApiResponse<IEnumerable<DropdownItemDto>>> GetCategoryOptionsByTypeAsync(string categoryTypeName);
    Task<ApiResponse<bool>> UpdateCategoryAsync(Category category);
    Task<ApiResponse<bool>> DeleteCategoryAsync(int categoryId);
}

public interface IAssignmentService
{
    Task<ApiResponse<bool>> AssignRoleToUserAsync(int userId, int roleId);
    Task<ApiResponse<bool>> RevokeRoleFromUserAsync(int userId, int roleId);
    Task<ApiResponse<bool>> SetUserSingleRoleAsync(int userId, int roleId);
    Task<ApiResponse<bool>> AssignMenuToRoleAsync(int roleId, int menuId);
    Task<ApiResponse<bool>> RevokeMenuFromRoleAsync(int roleId, int menuId);
    Task<ApiResponse<IEnumerable<Role>>> GetUserRolesAsync(int userId);
    Task<ApiResponse<IEnumerable<Menu>>> GetUserMenusAsync(int userId);
    Task<ApiResponse<IEnumerable<Menu>>> GetRoleMenusAsync(int roleId);
}
