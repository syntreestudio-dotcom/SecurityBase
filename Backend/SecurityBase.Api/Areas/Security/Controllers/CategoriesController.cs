using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Api.Controllers;

[Authorize]
[ApiController]
[Area("Security")]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var response = await _categoryService.GetCategoriesAsync();
        return Ok(response);
    }

    [HttpGet("compact")]
    public async Task<IActionResult> GetCategoryOptions([FromQuery] string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return BadRequest(new { message = "typeName is required." });
        }

        var response = await _categoryService.GetCategoryOptionsByTypeAsync(typeName);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] Category category)
    {
        var response = await _categoryService.CreateCategoryAsync(category);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] Category category)
    {
        var response = await _categoryService.UpdateCategoryAsync(category);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var response = await _categoryService.DeleteCategoryAsync(id);
        return Ok(response);
    }
}
