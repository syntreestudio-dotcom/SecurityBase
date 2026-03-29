using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Api.Controllers;

[Authorize]
[ApiController]
[Area("Security")]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var response = await _roleService.GetRolesAsync();
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] Role role)
    {
        var response = await _roleService.CreateRoleAsync(role);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole([FromBody] Role role)
    {
        var response = await _roleService.UpdateRoleAsync(role);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var response = await _roleService.DeleteRoleAsync(id);
        return Ok(response);
    }
}
