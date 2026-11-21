using Dapper;
using Microsoft.Data.SqlClient;

namespace AlumniConnect.Repositories
{
    public class DashboardStats
    {
        public int TotalProjects { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public int TotalUsers { get; set; }
    }

    public interface IDashboardRepository
    {
        Task<DashboardStats> GetSystemStatsAsync();
    }

    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AlumniConnectContext")!;
        }

        public async Task<DashboardStats> GetSystemStatsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM Projects) AS TotalProjects,
                    (SELECT COUNT(*) FROM Projects WHERE Status = 'Approved') AS ApprovedCount,
                    (SELECT COUNT(*) FROM Projects WHERE Status = 'Pending') AS PendingCount,
                    (SELECT COUNT(*) FROM AspNetUsers) AS TotalUsers";

            return await connection.QuerySingleAsync<DashboardStats>(sql);
        }
    }
}