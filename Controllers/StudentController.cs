using AlumniConnect.Models;
using AlumniConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlumniConnect.Controllers
{
    [Authorize(Roles = "Student,Admin")]
    public class StudentController : Controller
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IUserRepository _userRepo;
        private readonly IFileRepository _fileRepo;

        public StudentController(
            IProjectRepository projectRepo,
            IUserRepository userRepo,
            IFileRepository fileRepo)
        {
            _projectRepo = projectRepo;
            _userRepo = userRepo;
            _fileRepo = fileRepo;
        }

        public async Task<IActionResult> Index()
        {
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                var currentUser = await _userRepo.GetUserByEmailAsync(email);
                ViewBag.StudentName = currentUser?.FullName;
                ViewBag.StudentRoll = currentUser?.RollNumber;
            }

            var projects = await _projectRepo.GetAllProjectsAsync();
            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project, IFormFile? pdfFile)
        {
            if (ModelState.IsValid)
            {
                project.PdfFilePath = await _fileRepo.SavePdfAsync(pdfFile);
                project.Status = "Pending";
                project.SubmittedDate = DateTime.Now;

                await _projectRepo.AddAsync(project);
                return RedirectToAction(nameof(Index));
            }

            var projects = await _projectRepo.GetAllProjectsAsync();
            return View("Index", projects);
        }
    }
}