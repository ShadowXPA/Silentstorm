using CommonLib.Data;
using CommonLib.Models;
using CommonLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backoffice.Controllers
{
    [Authorize]
    [Route("vote")]
    public class VoteController : Controller
    {
        private readonly ILogger<VoteController> _logger;
        private readonly ProjectService _projectService;
        private readonly SilentstormUserService _silentstormUserService;

        public VoteController(ILogger<VoteController> logger, ProjectService projectService, SilentstormUserService silentstormUserService)
        {
            _logger = logger;
            _projectService = projectService;
            _silentstormUserService = silentstormUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var projects = await _projectService.GetAllProjectsByUserAndStatusAsync(userId, ProjectStatus.Status.Voting);
            var users = await _silentstormUserService.GetAllSilentStormUsersAsync();
            var submissions = new Dictionary<uint, List<SongSubmissionDto>>();
            foreach (var project in projects)
            {
                submissions.Add((uint)project.Id!, await _projectService.GetAllSongSubmissionsByProjectIdAndSelectedForVotingAsync((uint)project.Id));
            }
            return View((projects, users, submissions));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] SongVoteDto songVote)
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync((uint)songVote.Project!.Id!, userId, true);
            if (project is null) return View(false);

            var added = await _projectService.AddSongVoteAsync(songVote);
            return View(added);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] SongVoteDto songVote)
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync((uint)songVote.Project!.Id!, userId);

            if (project is null) return NotFound();

            var removed = await _projectService.RemoveSongVoteAsync((uint)songVote.Project.Id!, (uint)songVote.User!.Id!);
            return removed ? Ok() : BadRequest();
        }
    }
}
