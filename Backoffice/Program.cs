using Backoffice.Utils;
using CommonLib.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Backoffice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMvc()
                .AddNewtonsoftJson(newtonsoft =>
                {
                    newtonsoft.SerializerSettings.NullValueHandling = JsonExtension.JsonSerializerSettings.NullValueHandling;
                    newtonsoft.SerializerSettings.ContractResolver = JsonExtension.JsonSerializerSettings.ContractResolver;
                });
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);

                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    //options.AccessDeniedPath = "/error";
                    options.SlidingExpiration = true;
                });
            builder.Services.AddHttpClient();
            builder.Services.AddSilentStormServices();

            var app = builder.Build();

            if (builder.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UsePathBase(new PathString("/silentstorm"));
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
