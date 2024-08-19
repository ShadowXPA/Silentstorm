using CommonLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backoffice.Controllers
{
    [Authorize]
    [Route("type")]
    public class ProjectTypeController : Controller
    {
        private readonly ILogger<ProjectTypeController> _logger;
        private readonly ProjectTypeService _projectTypeService;

        public ProjectTypeController(ILogger<ProjectTypeController> logger, ProjectTypeService projectTypeService)
        {
            _logger = logger;
            _projectTypeService = projectTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var types = await _projectTypeService.GetAllProjectTypesAsync();
            return View(types);
        }
    }
}
