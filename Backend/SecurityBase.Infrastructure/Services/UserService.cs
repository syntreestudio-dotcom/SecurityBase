using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;

namespace SecurityBase.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<int>> CreateUserAsync(User user)
    {
        try
        {
            user.PasswordHash = PasswordHashing.HashPassword(user.PasswordHash);
            var userId = await _userRepository.CreateUserAsync(user);
            return new ApiResponse<int> { Success = true, Data = userId, Message = "User created successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<PaginatedList<UserDto>>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        try
        {
            var users = await _userRepository.GetUsersAsync(pageNumber, pageSize, searchTerm);
            var totalCount = users.Any() ? 100 : 0; // TODO: properly handle total count from SP if needed
            return new ApiResponse<PaginatedList<UserDto>> 
            { 
                Success = true, 
                Data = new PaginatedList<UserDto> { Items = users.ToList(), TotalCount = totalCount } 
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PaginatedList<UserDto>> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> UpdateUserAsync(User user)
    {
        try
        {
            await _userRepository.UpdateUserAsync(user);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "User updated successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> UpdateUserPasswordAsync(int userId, string newPassword)
    {
        try
        {
            var hash = PasswordHashing.HashPassword(newPassword);
            await _userRepository.UpdateUserPasswordAsync(userId, hash);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "Password updated successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(int userId)
    {
        try
        {
            await _userRepository.DeleteUserAsync(userId);
            return new ApiResponse<bool> { Success = true, Data = true, Message = "User deleted successfully" };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = ex.Message };
        }
    }
}
