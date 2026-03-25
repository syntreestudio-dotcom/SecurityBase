using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace SecurityBase.Infrastructure.Repositories;

public abstract class BaseRepository
{
    private readonly string _connectionString;

    protected BaseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("DefaultConnection string is missing");
    }

    protected IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
