using AlumniConnect.Data;
using AlumniConnect.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AlumniConnect.Repositories
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetApprovedProjectsAsync(string? search, string? domain);
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project?> GetByIdAsync(int id);
        Task AddAsync(Project project);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> DeleteAsync(int id);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly AlumniConnectContext _context;
        private readonly string _connectionString;

        public ProjectRepository(AlumniConnectContext context, IConfiguration configuration)
        {
            _context = context;
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

        public async Task<Project?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Project>("SELECT * FROM Projects WHERE Id = @Id", new { Id = id });
        }

        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            using var connection = new SqlConnection(_connectionString);
            var rows = await connection.ExecuteAsync("UPDATE Projects SET Status = @Status WHERE Id = @Id", new { Id = id, Status = status });
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var rows = await connection.ExecuteAsync("DELETE FROM Projects WHERE Id = @Id", new { Id = id });
            return rows > 0;
        }
    }
}