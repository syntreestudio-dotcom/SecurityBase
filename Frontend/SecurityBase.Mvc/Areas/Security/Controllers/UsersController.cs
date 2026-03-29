using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Mvc.Services;

namespace SecurityBase.Mvc.Controllers;

[Area("Security")]
public class UsersController : Controller
{
    private readonly IApiService _apiService;

    public UsersController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Users";
        ViewData["IsGridPage"] = true;
        if (HttpContext.Session.GetString("JWToken") == null)
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers(int draw = 1, int start = 0, int length = 1000, string searchParam = "")
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return Unauthorized(new { draw, recordsTotal = 0, recordsFiltered = 0, data = Array.Empty<UserDto>() });

        int pageNumber = length > 0 ? (start / length) + 1 : 1;
        ApiResponse<PaginatedList<UserDto>>? response;
        try
        {
            response = await _apiService.GetAsync<ApiResponse<PaginatedList<UserDto>>>($"users?pageNumber={pageNumber}&pageSize={length}&searchTerm={searchParam}");
        }
        catch
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { draw, recordsTotal = 0, recordsFiltered = 0, data = Array.Empty<UserDto>(), error = "Failed to reach API." });
        }

        if (response == null)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { draw, recordsTotal = 0, recordsFiltered = 0, data = Array.Empty<UserDto>(), error = "API returned no response (possible unauthorized or wrong BaseUrl)." });
        }
        
        if (response != null && response.Success && response.Data != null)
        {
            return Json(new { draw, recordsTotal = response.Data.TotalCount, recordsFiltered = response.Data.TotalCount, data = response.Data.Items });
        }
        return Json(new { draw, recordsTotal = 0, recordsFiltered = 0, data = new List<UserDto>() });
    }

    [HttpPost]
    public async Task<IActionResult> SaveUser(User user)
    {
        ApiResponse<int>? response;
        if (user.UserId == 0)
        {
            response = await _apiService.PostAsync<ApiResponse<int>, User>("users", user);
        }
        else
        {
            var newPassword = user.PasswordHash;
            user.PasswordHash = string.Empty; // never send password in the profile update call

            var updateResponse = await _apiService.PutAsync<ApiResponse<bool>, User>("users", user);
            if (updateResponse == null || !updateResponse.Success)
            {
                response = new ApiResponse<int> { Success = false, Message = updateResponse?.Message ?? "Update failed." };
                return Json(response);
            }

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                var pwdResp = await _apiService.PutAsync<ApiResponse<bool>, UserPasswordUpdateRequest>($"users/{user.UserId}/password", new UserPasswordUpdateRequest { Password = newPassword });
                if (pwdResp == null || !pwdResp.Success)
                {
                    response = new ApiResponse<int> { Success = false, Message = pwdResp?.Message ?? "Password update failed." };
                    return Json(response);
                }
                response = new ApiResponse<int> { Success = true, Message = "User updated and password changed successfully." };
            }
            else
            {
                response = new ApiResponse<int> { Success = true, Message = updateResponse.Message ?? "User updated successfully." };
            }
        }
        return Json(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var response = await _apiService.DeleteAsync<ApiResponse<bool>>($"users/{id}");
        return Json(response);
    }
}
