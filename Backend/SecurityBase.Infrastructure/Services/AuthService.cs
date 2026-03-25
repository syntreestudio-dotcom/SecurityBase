using SecurityBase.Core.DTOs;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IAssignmentRepository assignmentRepository, IJwtService jwtService)
    {
        _assignmentRepository = assignmentRepository;
        _jwtService = jwtService;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var (user, roles, menus) = await _assignmentRepository.GetUserPermissionsAsync(request.Username);

            if (user == null || !PasswordHashing.VerifyPassword(user.PasswordHash, request.Password))
            {
                return new ApiResponse<LoginResponse> { Success = false, Message = "Invalid username or password" };
            }

            if (!user.IsActive)
            {
                return new ApiResponse<LoginResponse> { Success = false, Message = "User account is inactive" };
            }

            var token = _jwtService.GenerateToken(user, roles);

            return new ApiResponse<LoginResponse>
            {
                Success = true,
                Data = new LoginResponse
                {
                    Token = token,
                    Username = user.Username,
                    Roles = roles.ToList(),
                    Menus = menus.Select(m => new MenuDto
                    {
                        MenuId = m.MenuId,
                        MenuName = m.MenuName,
                        ParentMenuId = m.ParentMenuId,
                        Route = m.Route,
                        Icon = m.Icon,
                        DisplayOrder = m.DisplayOrder
                    }).ToList()
                },
                Message = "Login successful"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<LoginResponse> { Success = false, Message = ex.Message };
        }
    }
}
