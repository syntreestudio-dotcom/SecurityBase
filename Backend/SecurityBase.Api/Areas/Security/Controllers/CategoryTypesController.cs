using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Api.Controllers;

[Authorize]
[ApiController]
[Area("Security")]
[Route("api/categorytypes")]
public class CategoryTypesController : ControllerBase
{
    private readonly ICategoryTypeService _categoryTypeService;

    public CategoryTypesController(ICategoryTypeService categoryTypeService)
    {
        _categoryTypeService = categoryTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoryTypes()
    {
        var response = await _categoryTypeService.GetCategoryTypesAsync();
        return Ok(response);
    }

    [HttpGet("compact")]
    public async Task<IActionResult> GetCategoryTypeOptions()
    {
        var response = await _categoryTypeService.GetCategoryTypeOptionsAsync();
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategoryType([FromBody] CategoryType categoryType)
    {
        var response = await _categoryTypeService.CreateCategoryTypeAsync(categoryType);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCategoryType([FromBody] CategoryType categoryType)
    {
        var response = await _categoryTypeService.UpdateCategoryTypeAsync(categoryType);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategoryType(int id)
    {
        var response = await _categoryTypeService.DeleteCategoryTypeAsync(id);
        return Ok(response);
    }
}
