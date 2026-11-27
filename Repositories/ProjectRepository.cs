using AlumniConnect.Data;
using AlumniConnect.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AlumniConnect.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<IEnumerable<Project>> GetApprovedProjectsAsync(string? search, string? domain);
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<bool> UpdateStatusAsync(int id, string status);
    }

    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private readonly string _connectionString;

        public ProjectRepository(AlumniConnectContext context, IConfiguration configuration)
            : base(context)
        {
            _connectionString = configuration.GetConnectionString("AlumniConnectContext")!;
        }

        public async Task<IEnumerable<Project>> GetApprovedProjectsAsync(string? search, string? domain)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Projects WHERE Status = 'Approved'";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " AND (Title LIKE @Search OR AuthorName LIKE @Search OR SupervisorName LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }

            if (!string.IsNullOrWhiteSpace(domain) && domain != "All")
            {
                sql += " AND Domain = @Domain";
                parameters.Add("Domain", domain);
            }

            sql += " ORDER BY SubmittedDate DESC";
            return await connection.QueryAsync<Project>(sql, parameters);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Project>("SELECT * FROM Projects ORDER BY SubmittedDate DESC");
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            using var connection = new SqlConnection(_connectionString);
            var rows = await connection.ExecuteAsync("UPDATE Projects SET Status = @Status WHERE Id = @Id", new { Id = id, Status = status });
            return rows > 0;
        }
    }
}