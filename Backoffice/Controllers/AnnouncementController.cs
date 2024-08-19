using CommonLib.Models;
using CommonLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backoffice.Controllers
{
    [Authorize]
    [Route("announcement")]
    public class AnnouncementController : Controller
    {
        private readonly ILogger<AnnouncementController> _logger;
        private readonly ProjectService _projectService;
        private readonly ProjectStatusService _projectStatusService;
        private readonly ChannelService _channelService;

        public AnnouncementController(ILogger<AnnouncementController> logger,
            ProjectService projectService,
            ProjectStatusService projectStatusService,
            ChannelService channelService)
        {
            _logger = logger;
            _projectService = projectService;
            _projectStatusService = projectStatusService;
            _channelService = channelService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var projects = await _projectService.GetAllProjectsByUserAsync(userId);
            var projectStatuses = await _projectStatusService.GetAllProjectStatusesAsync();
            var channels = await _channelService.GetAllChannelsAsync();
            return View((projects, projectStatuses, channels));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProjectAnnouncementDto projectAnnouncement)
        {
            if (!ModelState.IsValid) return View(false);

            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync((uint)projectAnnouncement.ProjectId!, userId);

            if (project is null) return View(false);

            var added = await _projectService.AddProjectAnnouncementAsync(projectAnnouncement);

            return View(added);
        }

        [HttpPost("reset")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset([FromForm] ProjectAnnouncementDto projectAnnouncement)
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync((uint)projectAnnouncement.ProjectId!, userId);

            if (project is null) return NotFound();

            var announcement = await _projectService.GetProjectAnnouncementAsync((uint)project.Id!, projectAnnouncement.ProjectStatus!);

            if (announcement is null) return NotFound();

            announcement.WasAnnounced = false;
            var updated = await _projectService.UpdateProjectAnnouncementAsync(announcement);

            return updated ? Ok() : BadRequest();
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] ProjectAnnouncementDto projectAnnouncement)
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var project = await _projectService.GetProjectByUserIdAsync((uint)projectAnnouncement.ProjectId!, userId);

            if (project is null) return NotFound();

            var removed = await _projectService.RemoveProjectAnnouncementAsync((uint)project.Id!, projectAnnouncement.ProjectStatus!);

            return removed ? Ok() : BadRequest();
        }
    }
}
