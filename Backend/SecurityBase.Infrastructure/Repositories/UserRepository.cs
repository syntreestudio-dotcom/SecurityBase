using Dapper;
using Microsoft.Extensions.Configuration;
using SecurityBase.Core.DTOs;
using SecurityBase.Core.Entities;
using SecurityBase.Core.Interfaces;
using System.Data;

namespace SecurityBase.Infrastructure.Repositories;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(IConfiguration configuration) : base(configuration) { }

    public async Task<int> CreateUserAsync(User user)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>("sp_CreateUser", 
            new { user.Username, user.PasswordHash, user.Email, user.IsActive }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync<UserDto>("sp_GetUsers", 
            new { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> UpdateUserAsync(User user)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync("sp_UpdateUser", 
            new { user.UserId, user.Username, user.Email, user.IsActive }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> UpdateUserPasswordAsync(int userId, string passwordHash)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync("sp_UpdateUserPassword",
            new { UserId = userId, PasswordHash = passwordHash },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> DeleteUserAsync(int userId)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteAsync("sp_DeleteUser", 
            new { UserId = userId }, 
            commandType: CommandType.StoredProcedure);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        using var connection = CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Username = @Username", 
            new { Username = username });
    }
}
