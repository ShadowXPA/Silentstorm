using CommonLib.Models;
using CommonLib.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backoffice.Controllers
{
    [Authorize]
    [Route("user")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly SilentstormUserService _silentstormUserService;

        public UserController(ILogger<UserController> logger, SilentstormUserService silentstormUserService)
        {
            _logger = logger;
            _silentstormUserService = silentstormUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _silentstormUserService.GetAllSilentStormUsersAsync();
            return View(users);
        }

        [HttpGet("new")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Created([FromForm] SilentstormUserDto silentstormUser)
        {
            if (!ModelState.IsValid) return View(false);
            silentstormUser.DiscordId = string.Empty;
            var added = await _silentstormUserService.AddSilentStormUserAsync(silentstormUser);
            return View(added);
        }

        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            if (id <= 2) return NotFound();
            var removed = await _silentstormUserService.RemoveSilentStormUserAsync(id);
            return removed ? Ok() : BadRequest();
        }
    }
}
