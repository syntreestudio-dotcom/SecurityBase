using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Mvc.Services;

namespace SecurityBase.Mvc.Controllers;

[Area("Security")]
public class RolesController : Controller
{
    private readonly IApiService _apiService;

    public RolesController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return RedirectToAction("Login", "Account");

        ViewData["ActivePage"] = "Roles";
        ViewData["IsGridPage"] = true;
        
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return Unauthorized(new { data = Array.Empty<RoleDto>() });

        ApiResponse<IEnumerable<RoleDto>>? response;
        try
        {
            response = await _apiService.GetAsync<ApiResponse<IEnumerable<RoleDto>>>("roles");
        }
        catch
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<RoleDto>(), error = "Failed to reach API." });
        }

        if (response == null)
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<RoleDto>(), error = "API returned no response." });

        if (response != null && response.Success)
        {
            return Json(new { data = response.Data });
        }
        return Json(new { data = new List<RoleDto>() });
    }

    [HttpPost]
    public async Task<IActionResult> SaveRole(Role role)
    {
        ApiResponse<int>? response;
        if (role.RoleId == 0)
        {
            response = await _apiService.PostAsync<ApiResponse<int>, Role>("roles", role);
        }
        else
        {
            var updateResponse = await _apiService.PutAsync<ApiResponse<bool>, Role>("roles", role);
            response = new ApiResponse<int> { Success = updateResponse?.Success ?? false, Message = updateResponse?.Message };
        }
        return Json(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var response = await _apiService.DeleteAsync<ApiResponse<bool>>($"roles/{id}");
        return Json(response);
    }
}
