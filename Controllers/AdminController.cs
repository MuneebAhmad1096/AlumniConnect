using AlumniConnect.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlumniConnect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AlumniConnectContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(AlumniConnectContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .OrderByDescending(p => p.SubmittedDate)
                .ToListAsync();

            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalProjects = projects.Count;
            ViewBag.ApprovedCount = projects.Count(p => p.Status == "Approved");
            ViewBag.PendingCount = projects.Count(p => p.Status == "Pending");

            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                if (!string.IsNullOrEmpty(project.PdfFilePath))
                {
                    string fullPath = Path.Combine(_env.WebRootPath, project.PdfFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = $"Project '{project.Title}' was permanently deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}