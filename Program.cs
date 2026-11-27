using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AlumniConnect.Data;
using AlumniConnect.Repositories;

namespace AlumniConnect
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("AlumniConnectContext")
                ?? throw new InvalidOperationException("Connection string 'AlumniConnectContext' not found.");

            builder.Services.AddDbContext<AlumniConnectContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
                options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AlumniConnectContext>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IFileRepository, FileRepository>();

            var app = builder.Build();

            // Seed default roles and demo accounts
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roles = { "Admin", "Faculty", "Student" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                async Task SeedUser(string email, string password, string role, string fullName, string? rollNumber = null)
                {
                    if (await userManager.FindByEmailAsync(email) == null)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            EmailConfirmed = true,
                            FullName = fullName,
                            RollNumber = rollNumber,
                            Department = "Software Engineering"
                        };
                        var result = await userManager.CreateAsync(user, password);
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, role);
                        }
                    }
                }

                await SeedUser("admin@alumniconnect.edu.pk", "Admin@123", "Admin", "System Administrator");
                await SeedUser("faculty@alumniconnect.edu.pk", "Faculty@123", "Faculty", "Dr. Shuja ur Rehman");
                await SeedUser("student@alumniconnect.edu.pk", "Student@123", "Student", "Muneeb Ahmad", "BSEF24M012");
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}