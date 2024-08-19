using CommonLib.Data;
using CommonLib.Models;
using CommonLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backoffice.Controllers
{
    [Authorize]
    [Route("project")]
    public class ProjectController : Controller
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly ProjectService _projectService;
        private readonly ProjectTypeService _projectTypeService;
        private readonly ProjectStatusService _projectStatusService;

        public ProjectController(ILogger<ProjectController> logger,
            ProjectService projectService,
            ProjectTypeService projectTypeService,
            ProjectStatusService projectStatusService)
        {
            _logger = logger;
            _projectService = projectService;
            _projectTypeService = projectTypeService;
            _projectStatusService = projectStatusService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var projectTypes = await _projectTypeService.GetAllProjectTypesAsync();
            return View(projectTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProjectDto project)
        {
            if (!ModelState.IsValid) return View(false);

            var claims = User.Claims;

            project.User = new()
            {
                Id = uint.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
                Username = claims.First(c => c.Type == ClaimTypes.Name).Value,
                DiscordId = claims.First(c => c.Type == ClaimTypes.Upn).Value
            };

            var added = await _projectService.AddProjectAsync(project);

            return View(added);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(uint id)
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync(id, userId, true);
            if (project is null) return NotFound();
            var projectAnnouncements = await _projectService.GetProjectAnnouncementsAsync(project.Id, true);
            var projectSubmissions = await _projectService.GetAllSongSubmissionsByProjectIdAsync((uint)project.Id!, true);
            var projectVotes = await _projectService.GetAllSongVotesByProjectIdAsync((uint)project.Id, true);
            return View((project, projectAnnouncements, projectSubmissions, projectVotes));
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync(id, userId, true);
            if (project is null) return NotFound();
            var deleted = await _projectService.RemoveProjectAsync(id);
            return deleted ? Ok() : BadRequest();
        }

        [HttpPost("{id}/advance")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Advance(uint id)
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync(id, userId, true);
            if (project is null) return NotFound();

            project.ProjectStatus = _projectStatusService.FindNextStatus(project.ProjectStatus);

            switch (project.ProjectStatus)
            {
                case ProjectStatus.Status.Voting:
                    {
                        var songSubmissions = await _projectService.GetAllSongSubmissionsByProjectIdAsync(id);
                        var count = songSubmissions.Count;

                        if (count > 4)
                        {
                            for (int i = 0; i < 4;)
                            {
                                var selectedIndex = Random.Shared.Next(0, count - 1);
                                if (songSubmissions[selectedIndex].IsSelectedForVoting ?? false) continue;
                                songSubmissions[selectedIndex].IsSelectedForVoting = true;
                                await _projectService.UpdateSongSubmissionAsync(songSubmissions[selectedIndex]);
                            }
                        }
                        else
                        {
                            foreach (var songSubmission in songSubmissions)
                            {
                                songSubmission.IsSelectedForVoting = true;
                                await _projectService.UpdateSongSubmissionAsync(songSubmission);
                            }
                        }
                        break;
                    }
                case ProjectStatus.Status.Finished:
                    {
                        project.FinishedAt = DateTime.Now;
                        break;
                    }
            }

            if (project.ProjectStatus == ProjectStatus.Status.Finished)
            {
                project.FinishedAt = DateTime.Now;
            }

            var updated = await _projectService.UpdateProjectAsync(project);

            return updated ? Ok() : BadRequest();
        }
    }
}
