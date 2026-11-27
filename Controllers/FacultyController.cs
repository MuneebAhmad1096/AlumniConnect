using AlumniConnect.Hubs;
using AlumniConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace AlumniConnect.Controllers
{
    [Authorize(Roles = "Faculty,Admin")]
    public class FacultyController : Controller
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMemoryCache _cache;

        public FacultyController(
            IProjectRepository projectRepo,
            IHubContext<NotificationHub> hubContext,
            IMemoryCache cache)
        {
            _projectRepo = projectRepo;
            _hubContext = hubContext;
            _cache = cache;
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

            if (updated)
            {
                _cache.Remove("ApprovedProjects_All_");

                if (status == "Approved")
                {
                    var project = await _projectRepo.GetByIdAsync(id);
                    if (project != null)
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveProjectAlert", project.Title, project.Domain);
                    }
                }
            }

            return Json(new { success = updated, newStatus = status });
        }
    }
}