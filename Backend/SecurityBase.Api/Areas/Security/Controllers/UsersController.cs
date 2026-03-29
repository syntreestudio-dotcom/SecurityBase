using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Api.Controllers;

[Authorize]
[ApiController]
[Area("Security")]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        var response = await _userService.GetUsersAsync(pageNumber, pageSize, searchTerm);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var response = await _userService.CreateUserAsync(user);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] User user)
    {
        var response = await _userService.UpdateUserAsync(user);
        return Ok(response);
    }

    [HttpPut("{id}/password")]
    public async Task<IActionResult> UpdatePassword(int id, [FromBody] UserPasswordUpdateRequest request)
    {
        var response = await _userService.UpdateUserPasswordAsync(id, request.Password);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var response = await _userService.DeleteUserAsync(id);
        return Ok(response);
    }
}
