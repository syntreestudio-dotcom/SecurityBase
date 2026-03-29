using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Api.Controllers;

[Authorize]
[ApiController]
[Area("Security")]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromQuery] int userId, [FromQuery] int roleId)
    {
        var response = await _assignmentService.AssignRoleToUserAsync(userId, roleId);
        return Ok(response);
    }

    [HttpPost("revoke-role")]
    public async Task<IActionResult> RevokeRole([FromQuery] int userId, [FromQuery] int roleId)
    {
        var response = await _assignmentService.RevokeRoleFromUserAsync(userId, roleId);
        return Ok(response);
    }

    [HttpPost("set-user-role")]
    public async Task<IActionResult> SetUserRole([FromQuery] int userId, [FromQuery] int roleId)
    {
        var response = await _assignmentService.SetUserSingleRoleAsync(userId, roleId);
        return Ok(response);
    }

    [HttpPost("assign-role-menu")]
    public async Task<IActionResult> AssignRoleMenu([FromQuery] int roleId, [FromQuery] int menuId)
    {
        var response = await _assignmentService.AssignMenuToRoleAsync(roleId, menuId);
        return Ok(response);
    }

    [HttpPost("revoke-role-menu")]
    public async Task<IActionResult> RevokeRoleMenu([FromQuery] int roleId, [FromQuery] int menuId)
    {
        var response = await _assignmentService.RevokeMenuFromRoleAsync(roleId, menuId);
        return Ok(response);
    }

    [HttpGet("user-roles/{userId}")]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        var response = await _assignmentService.GetUserRolesAsync(userId);
        return Ok(response);
    }

    [HttpGet("user-menus/{userId}")]
    public async Task<IActionResult> GetUserMenus(int userId)
    {
        var response = await _assignmentService.GetUserMenusAsync(userId);
        return Ok(response);
    }

    [HttpGet("role-menus/{roleId}")]
    public async Task<IActionResult> GetRoleMenus(int roleId)
    {
        var response = await _assignmentService.GetRoleMenusAsync(roleId);
        return Ok(response);
    }
}
