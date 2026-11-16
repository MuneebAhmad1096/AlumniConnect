using AlumniConnect.Data;
using AlumniConnect.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AlumniConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly AlumniConnectContext _context;

        public HomeController(AlumniConnectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, string? domain)
        {
            var query = _context.Projects
                .Where(p => p.Status == "Approved")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Title.Contains(search) ||
                                         p.AuthorName.Contains(search) ||
                                         p.SupervisorName.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(domain) && domain != "All")
            {
                query = query.Where(p => p.Domain == domain);
            }

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentDomain = domain ?? "All";

            var approvedProjects = await query
                .OrderByDescending(p => p.SubmittedDate)
                .ToListAsync();

            return View(approvedProjects);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}