using AlumniConnect.Data;
using AlumniConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlumniConnect.Controllers
{
    [Authorize(Roles = "Student,Admin")]
    public class StudentController : Controller
    {
        private readonly AlumniConnectContext _context;
        private readonly IWebHostEnvironment _env;

        public StudentController(AlumniConnectContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .OrderByDescending(p => p.SubmittedDate)
                .ToListAsync();

            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitProject(Project project, IFormFile? pdfFile)
        {
            if (pdfFile != null && pdfFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "docs");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(pdfFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                project.PdfFilePath = "/docs/" + uniqueFileName;
            }

            project.Status = "Pending";
            project.SubmittedDate = DateTime.Now;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Project submitted successfully and is awaiting Faculty approval!";
            return RedirectToAction(nameof(Index));
        }
    }
}