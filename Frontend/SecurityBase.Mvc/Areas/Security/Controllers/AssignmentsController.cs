using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Mvc.Services;

namespace SecurityBase.Mvc.Controllers;

[Area("Security")]
public class AssignmentsController : Controller
{
    private readonly IApiService _apiService;

    public AssignmentsController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> UserRoles()
    {
        ViewData["ActivePage"] = "Assignments"; // Moved to the beginning
        if (HttpContext.Session.GetString("JWToken") == null)
            return RedirectToAction("Login", "Account");

        var usersResponse = await _apiService.GetAsync<ApiResponse<PaginatedList<UserDto>>>("users?pageSize=100");
        var rolesResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<RoleDto>>>("roles");

        ViewBag.Users = usersResponse?.Data?.Items ?? new List<UserDto>();
        ViewBag.Roles = rolesResponse?.Data ?? new List<RoleDto>();

        return View();
    }

    public async Task<IActionResult> UserMenus()
    {
        return RedirectToAction(nameof(RoleMenus));
    }

    public async Task<IActionResult> RoleMenus()
    {
        ViewData["ActivePage"] = "Assignments"; // Moved to the beginning
        if (HttpContext.Session.GetString("JWToken") == null)
            return RedirectToAction("Login", "Account");

        var rolesResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<RoleDto>>>("roles");
        var menusResponse = await _apiService.GetAsync<ApiResponse<IEnumerable<Menu>>>("menus");

        ViewBag.Roles = rolesResponse?.Data ?? new List<RoleDto>();
        ViewBag.Menus = menusResponse?.Data ?? new List<Menu>();

        return View("RoleMenus");
    }

    [HttpGet]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        var response = await _apiService.GetAsync<ApiResponse<IEnumerable<Role>>>($"assignments/user-roles/{userId}");
        return Json(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserMenus(int userId)
    {
        var response = await _apiService.GetAsync<ApiResponse<IEnumerable<Menu>>>($"assignments/user-menus/{userId}");
        return Json(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetRoleMenus(int roleId)
    {
        var response = await _apiService.GetAsync<ApiResponse<IEnumerable<Menu>>>($"assignments/role-menus/{roleId}");
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> AssignRole(int userId, int roleId)
    {
        var response = await _apiService.PostAsync<ApiResponse<bool>, object>($"assignments/assign-role?userId={userId}&roleId={roleId}", new { });
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> RevokeRole(int userId, int roleId)
    {
        var response = await _apiService.PostAsync<ApiResponse<bool>, object>($"assignments/revoke-role?userId={userId}&roleId={roleId}", new { });
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> SetUserRole(int userId, int roleId)
    {
        var response = await _apiService.PostAsync<ApiResponse<bool>, object>($"assignments/set-user-role?userId={userId}&roleId={roleId}", new { });
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> AssignRoleMenu(int roleId, int menuId)
    {
        var response = await _apiService.PostAsync<ApiResponse<bool>, object>($"assignments/assign-role-menu?roleId={roleId}&menuId={menuId}", new { });
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> RevokeRoleMenu(int roleId, int menuId)
    {
        var response = await _apiService.PostAsync<ApiResponse<bool>, object>($"assignments/revoke-role-menu?roleId={roleId}&menuId={menuId}", new { });
        return Json(response);
    }
}
