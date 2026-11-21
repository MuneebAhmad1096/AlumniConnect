using AlumniConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlumniConnect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IDashboardRepository _dashboardRepo;
        private readonly IUserRepository _userRepo;
        private readonly IFileRepository _fileRepo;

        public AdminController(
            IProjectRepository projectRepo,
            IDashboardRepository dashboardRepo,
            IUserRepository userRepo,
            IFileRepository fileRepo)
        {
            _projectRepo = projectRepo;
            _dashboardRepo = dashboardRepo;
            _userRepo = userRepo;
            _fileRepo = fileRepo;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Stats = await _dashboardRepo.GetSystemStatsAsync();
            ViewBag.Users = await _userRepo.GetAllUsersAsync();
            var projects = await _projectRepo.GetAllProjectsAsync();
            return View(projects);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _projectRepo.GetByIdAsync(id);
            if (project == null)
                return Json(new { success = false, message = "Project not found." });

            _fileRepo.DeletePdf(project.PdfFilePath);
            bool deleted = await _projectRepo.DeleteAsync(id);
            var updatedStats = await _dashboardRepo.GetSystemStatsAsync();

            return Json(new
            {
                success = deleted,
                stats = updatedStats
            });
        }
    }
}