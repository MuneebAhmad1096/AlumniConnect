using AlumniConnect.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AlumniConnect.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
    }

    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AlumniConnectContext")!;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ApplicationUser>(
                "SELECT Id, Email, FullName, RollNumber, Department FROM AspNetUsers");
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT Id, Email, FullName, RollNumber, Department FROM AspNetUsers WHERE Email = @Email",
                new { Email = email });
        }
    }
}