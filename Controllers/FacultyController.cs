using AlumniConnect.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlumniConnect.Controllers
{
    [Authorize(Roles = "Faculty,Admin")]
    public class FacultyController : Controller
    {
        private readonly AlumniConnectContext _context;

        public FacultyController(AlumniConnectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .OrderBy(p => p.Status == "Pending" ? 0 : 1)
                .ThenByDescending(p => p.SubmittedDate)
                .ToListAsync();

            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null && (status == "Approved" || status == "Rejected"))
            {
                project.Status = status;
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"Project '{project.Title}' has been marked as {status}.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}