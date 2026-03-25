using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMenus()
    {
        var response = await _menuService.GetMenusAsync();
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMenu([FromBody] Menu menu)
    {
        var response = await _menuService.CreateMenuAsync(menu);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMenu([FromBody] Menu menu)
    {
        var response = await _menuService.UpdateMenuAsync(menu);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenu(int id)
    {
        var response = await _menuService.DeleteMenuAsync(id);
        return Ok(response);
    }
}
