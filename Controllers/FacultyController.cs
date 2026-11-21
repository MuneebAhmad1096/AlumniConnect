using AlumniConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlumniConnect.Controllers
{
    [Authorize(Roles = "Faculty,Admin")]
    public class FacultyController : Controller
    {
        private readonly IProjectRepository _projectRepo;

        public FacultyController(IProjectRepository projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _projectRepo.GetAllProjectsAsync();
            return View(projects);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (status != "Approved" && status != "Rejected")
                return Json(new { success = false, message = "Invalid status." });

            bool updated = await _projectRepo.UpdateStatusAsync(id, status);
            return Json(new { success = updated, newStatus = status });
        }
    }
}