using CommonLib.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backoffice.Filter
{
    public class SilentstormAuthenticationFilter : IAsyncActionFilter
    {
        private readonly PropertiesService _propertiesService;

        public SilentstormAuthenticationFilter(PropertiesService propertiesService)
        {
            _propertiesService = propertiesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var isAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated;

            if (isAuthenticated is null || !(bool)isAuthenticated)
            {
                context.Result = new RedirectResult(_propertiesService.GetProperty("silentstorm.oauth2")!);
                return;
            }

            await next();
        }
    }
}
