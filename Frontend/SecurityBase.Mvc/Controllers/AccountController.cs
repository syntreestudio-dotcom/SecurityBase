using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.DTOs;
using SecurityBase.Mvc.Services;
using System.Text.Json;

namespace SecurityBase.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly IApiService _apiService;

    public AccountController(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("JWToken") != null)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _apiService.PostAsync<ApiResponse<LoginResponse>, LoginRequest>("auth/login", request);

        if (response != null && response.Success && response.Data != null)
        {
            HttpContext.Session.SetString("JWToken", response.Data.Token);
            HttpContext.Session.SetString("Username", response.Data.Username);
            HttpContext.Session.SetString("UserRoles", JsonSerializer.Serialize(response.Data.Roles ?? new List<string>()));
            HttpContext.Session.SetString("UserMenus", JsonSerializer.Serialize(response.Data.Menus ?? new List<MenuDto>()));
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = response?.Message ?? "Invalid login attempt";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
