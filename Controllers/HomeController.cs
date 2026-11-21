using AlumniConnect.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AlumniConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProjectRepository _projectRepo;

        public HomeController(IProjectRepository projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public async Task<IActionResult> Index(string? search, string? domain)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentDomain = domain ?? "All";
            var projects = await _projectRepo.GetApprovedProjectsAsync(search, domain);
            return View(projects);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProjects(string? search, string? domain)
        {
            var projects = await _projectRepo.GetApprovedProjectsAsync(search, domain);
            return PartialView("_ProjectCardsPartial", projects);
        }

        public IActionResult Privacy() => View();
    }
}