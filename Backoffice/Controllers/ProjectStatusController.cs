using CommonLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backoffice.Controllers
{
    [Authorize]
    [Route("status")]
    public class ProjectStatusController : Controller
    {
        private readonly ILogger<ProjectStatusController> _logger;
        private readonly ProjectStatusService _projectStatusService;

        public ProjectStatusController(ILogger<ProjectStatusController> logger, ProjectStatusService projectStatusService)
        {
            _logger = logger;
            _projectStatusService = projectStatusService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var statuses = await _projectStatusService.GetAllProjectStatusesAsync();
            return View(statuses);
        }
    }
}
