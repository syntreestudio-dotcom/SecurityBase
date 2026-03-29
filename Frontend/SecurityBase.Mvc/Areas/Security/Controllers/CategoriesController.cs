using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Mvc.Services;

namespace SecurityBase.Mvc.Controllers;

[Area("Security")]
public class CategoriesController : Controller
{
    private readonly IApiService _apiService;

    public CategoriesController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public IActionResult Index()
    {
        ViewData["ActivePage"] = "Categories";
        ViewData["IsGridPage"] = true;
        if (HttpContext.Session.GetString("JWToken") == null)
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return Unauthorized(new { data = Array.Empty<CategoryDto>() });

        ApiResponse<IEnumerable<CategoryDto>>? response;
        try
        {
            response = await _apiService.GetAsync<ApiResponse<IEnumerable<CategoryDto>>>("categories");
        }
        catch
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<CategoryDto>(), error = "Failed to reach API." });
        }

        if (response == null || !response.Success || response.Data == null)
            return StatusCode(StatusCodes.Status502BadGateway, new { data = Array.Empty<CategoryDto>(), error = response?.Message ?? "API returned no response." });

        return Json(new { data = response.Data });
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoryOptions(string typeName)
    {
        if (HttpContext.Session.GetString("JWToken") == null)
            return Unauthorized(new { data = Array.Empty<DropdownItemDto>() });

        if (string.IsNullOrWhiteSpace(typeName))
            return BadRequest(new { message = "typeName is required." });

        ApiResponse<IEnumerable<DropdownItemDto>>? response;
        try
        {
            response = await _apiService.GetAsync<ApiResponse<IEnumerable<DropdownItemDto>>>($"categories/compact?typeName={Uri.EscapeDataString(typeName)}");
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
    public async Task<IActionResult> SaveCategory(Category category)
    {
        if (category.CategoryTypeId <= 0)
        {
            return Json(new ApiResponse<int> { Success = false, Message = "Category type is required." });
        }

        ApiResponse<int>? response;
        if (category.CategoryId == 0)
        {
            response = await _apiService.PostAsync<ApiResponse<int>, Category>("categories", category);
        }
        else
        {
            var updateResponse = await _apiService.PutAsync<ApiResponse<bool>, Category>("categories", category);
            response = new ApiResponse<int>
            {
                Success = updateResponse != null && updateResponse.Success,
                Message = updateResponse?.Message ?? "Update failed."
            };
        }
        return Json(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var response = await _apiService.DeleteAsync<ApiResponse<bool>>($"categories/{id}");
        return Json(response);
    }
}
