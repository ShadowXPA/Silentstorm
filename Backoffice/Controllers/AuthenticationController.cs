using Backoffice.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CommonLib.Services;

namespace Backoffice.Controllers
{
    [AllowAnonymous]
    [Route("{action}")]
    public class AuthenticationController : Controller
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly PropertiesService _propertiesService;
        private readonly OAuth2Service _oauth2Service;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthenticationController(ILogger<AuthenticationController> logger,
            PropertiesService propertiesService,
            OAuth2Service oauth2Service,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _propertiesService = propertiesService;
            _oauth2Service = oauth2Service;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(_propertiesService.GetPropertyOrDefault($@"{(_webHostEnvironment.IsDevelopment() ? "dev." : "")}silentstorm.oauth2", "https://discord.com"));
        }

        public async Task<IActionResult> Authorize(string code)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var accessToken = (await _oauth2Service.GetAccessTokenAsync(code))!;
            var user = (await _oauth2Service.GetCurrentUserAsync(accessToken))!;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, $"{user.Id}"),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Upn, user.DiscordId ?? ""),
                new Claim(ClaimTypes.Authentication, accessToken.AccessToken)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddSeconds(accessToken.ExpiresIn)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
            authProperties);

            return Redirect(Url.Content("~/"));
        }

        public async Task<IActionResult> Logout()
        {
            var authentication = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Authentication);
            await _oauth2Service.RevokeAccessTokenAsync(authentication?.Value);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(Url.Content("~/"));
        }
    }
}
