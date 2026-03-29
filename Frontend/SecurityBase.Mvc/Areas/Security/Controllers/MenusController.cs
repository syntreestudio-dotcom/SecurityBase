using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Mvc.Services;

namespace SecurityBase.Mvc.Controllers;

[Area("Security")]
public class MenusController : Controller
{
    private readonly IApiService _apiService;

    public MenusController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Menus";
        ViewData["IsGridPage"] = true;
        if (HttpContext.Session.GetString("JWToken") == null)
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMenus()
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return Unauthorized(new { data = Array.Empty<Menu>() });

        ApiResponse<IEnumerable<Menu>>? response;
        try
        {
            response = await _apiService.GetAsync<ApiResponse<IEnumerable<Menu>>>("menus");
        }
        catch
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<Menu>(), error = "Failed to reach API." });
        }

        if (response == null)
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<Menu>(), error = "API returned no response." });

        if (response != null && response.Success)
        {
            return Json(new { data = response.Data });
        }
        return Json(new { data = new List<Menu>() });
    }

    [HttpPost]
    public async Task<IActionResult> SaveMenu(Menu menu)
    {
        ApiResponse<int>? response;
        if (menu.MenuId == 0)
        {
            response = await _apiService.PostAsync<ApiResponse<int>, Menu>("menus", menu);
        }
        else
        {
            var updateResponse = await _apiService.PutAsync<ApiResponse<bool>, Menu>("menus", menu);
            response = new ApiResponse<int> { Success = updateResponse?.Success ?? false, Message = updateResponse?.Message };
        }
        return Json(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMenu(int id)
    {
        var response = await _apiService.DeleteAsync<ApiResponse<bool>>($"menus/{id}");
        return Json(response);
    }
}
