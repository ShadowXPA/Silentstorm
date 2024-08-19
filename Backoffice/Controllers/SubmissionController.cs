using CommonLib.Data;
using CommonLib.Models;
using CommonLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backoffice.Controllers
{
    [Authorize]
    [Route("submission")]
    public class SubmissionController : Controller
    {
        private readonly ILogger<SubmissionController> _logger;
        private readonly ProjectService _projectService;
        private readonly SilentstormUserService _silentstormUserService;

        public SubmissionController(ILogger<SubmissionController> logger, ProjectService projectService, SilentstormUserService silentstormUserService)
        {
            _logger = logger;
            _projectService = projectService;
            _silentstormUserService = silentstormUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var projects = await _projectService.GetAllProjectsByUserAndStatusAsync(userId, ProjectStatus.Status.Submission);
            var users = await _silentstormUserService.GetAllSilentStormUsersAsync();
            return View((projects, users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] SongSubmissionDto songSubmission)
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync((uint)songSubmission.Project!.Id!, userId, true);
            if (project is null) return View(false);

            var added = await _projectService.AddSongSubmissionAsync(songSubmission);
            return View(added);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] SongSubmissionDto songSubmission)
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync((uint)songSubmission.Project!.Id!, userId);

            if (project is null) return NotFound();

            var removed = await _projectService.RemoveSongSubmissionAsync((uint)songSubmission.Id!);

            return removed ? Ok() : BadRequest();
        }
    }
}
