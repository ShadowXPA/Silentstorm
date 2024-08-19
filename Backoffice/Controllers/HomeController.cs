using CommonLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backoffice.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProjectService _projectService;

        public HomeController(ILogger<HomeController> logger, ProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = uint.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var projects = await _projectService.GetAllProjectsByUserAsync(userId);
            return View(projects);
        }
    }
}
