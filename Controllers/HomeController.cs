using AlumniConnect.Models;
using AlumniConnect.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AlumniConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IMemoryCache _cache;

        public HomeController(IProjectRepository projectRepo, IMemoryCache cache)
        {
            _projectRepo = projectRepo;
            _cache = cache;
        }

        public async Task<IActionResult> Index(string? search, string? domain)
        {
            var activeDomain = domain ?? HttpContext.Session.GetString("LastDomain") ?? "All";
            var activeSearch = search ?? HttpContext.Session.GetString("LastSearch") ?? "";

            ViewBag.CurrentDomain = activeDomain;
            ViewBag.CurrentSearch = activeSearch;

            var projects = await GetCachedProjectsAsync(activeSearch, activeDomain);
            return View(projects);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProjects(string? search, string? domain)
        {
            if (!string.IsNullOrWhiteSpace(domain))
            {
                HttpContext.Session.SetString("LastDomain", domain);
            }
            if (search != null)
            {
                HttpContext.Session.SetString("LastSearch", search);
            }

            var projects = await GetCachedProjectsAsync(search ?? "", domain ?? "All");
            return PartialView("_ProjectCardsPartial", projects);
        }

        private async Task<IEnumerable<Project>> GetCachedProjectsAsync(string search, string domain)
        {
            string cacheKey = $"ApprovedProjects_{domain}_{search.ToLowerInvariant()}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Project>? projects) || projects == null)
            {
                projects = await _projectRepo.GetApprovedProjectsAsync(
                    string.IsNullOrWhiteSpace(search) ? null : search,
                    domain);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

                _cache.Set(cacheKey, projects, cacheOptions);
            }

            return projects;
        }

        public IActionResult Privacy() => View();
    }
}