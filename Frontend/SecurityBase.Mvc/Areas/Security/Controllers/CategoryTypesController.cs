using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Mvc.Services;

namespace SecurityBase.Mvc.Controllers;

[Area("Security")]
public class CategoryTypesController : Controller
{
    private readonly IApiService _apiService;

    public CategoryTypesController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Category Types";
        ViewData["IsGridPage"] = true;
        if (HttpContext.Session.GetString("JWToken") == null)
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategoryTypes()
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return Unauthorized(new { data = Array.Empty<CategoryType>() });

        ApiResponse<IEnumerable<CategoryType>>? response;
        try
        {
            response = await _apiService.GetAsync<ApiResponse<IEnumerable<CategoryType>>>("categorytypes");
        }
        catch
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<CategoryType>(), error = "Failed to reach API." });
        }

        if (response == null || !response.Success || response.Data == null)
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<CategoryType>(), error = response?.Message ?? "API returned no response." });

        return Json(new { data = response.Data });
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoryTypeOptions()
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return Unauthorized(new { data = Array.Empty<DropdownItemDto>() });

        ApiResponse<IEnumerable<DropdownItemDto>>? response;
        try
        {
            response = await _apiService.GetAsync<ApiResponse<IEnumerable<DropdownItemDto>>>("categorytypes/compact");
        }
        catch
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<DropdownItemDto>(), error = "Failed to reach API." });
        }

        if (response == null || !response.Success || response.Data == null)
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<DropdownItemDto>(), error = response?.Message ?? "API returned no response." });

        return Json(new { data = response.Data });
    }

    [HttpPost]
    public async Task<IActionResult> SaveCategoryType(CategoryType categoryType)
    {
        ApiResponse<int>? response;
        if (categoryType.CategoryTypeId == 0)
        {
            response = await _apiService.PostAsync<ApiResponse<int>, CategoryType>("categorytypes", categoryType);
        }
        else
        {
            var updateResponse = await _apiService.PutAsync<ApiResponse<bool>, CategoryType>("categorytypes", categoryType);
            response = new ApiResponse<int>
            {
                Success = updateResponse != null && updateResponse.Success,
                Message = updateResponse?.Message ?? "Update failed."
            };
        }
        return Json(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCategoryType(int id)
    {
        var response = await _apiService.DeleteAsync<ApiResponse<bool>>($"categorytypes/{id}");
        return Json(response);
    }
}
