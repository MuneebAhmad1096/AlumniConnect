using AlumniConnect.Data;
using AlumniConnect.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlumniConnect.Data;

public class AlumniConnectContext : IdentityDbContext<ApplicationUser>
{
    public AlumniConnectContext(DbContextOptions<AlumniConnectContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}