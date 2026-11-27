using AlumniConnect.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AlumniConnect.ViewComponents
{
    public class RecentProjectsViewComponent : ViewComponent
    {
        private readonly IProjectRepository _projectRepository;

        public RecentProjectsViewComponent(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 3)
        {
            var approvedProjects = await _projectRepository.GetApprovedProjectsAsync(null, null);
            var recentProjects = approvedProjects.Take(count).ToList();
            return View(recentProjects);
        }
    }
}