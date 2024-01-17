using Newtonsoft.Json;
using Web.Extensions;

namespace Web.Middleware
{
    public class AppAuthenticate
    {
        private readonly RequestDelegate _next;
        private readonly LinkGenerator _linkGenerator;

        public AppAuthenticate(
            RequestDelegate next,
            LinkGenerator linkGenerator)
        {
            _next = next;
            _linkGenerator = linkGenerator;
        }

        public async Task InvokeAsync(HttpContext context)
        {  
            var user = context.Session.GetAccountProfile();

            if (user == null)
            {
                context.Response.Redirect("/Admin/Account/Login");
                return;
            } 

            await _next(context);
        }
    }

    public static class AppAuthenticateMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppAuthenticate(this IApplicationBuilder builder)
          => builder.UseMiddleware<AppAuthenticate>();
    }
}
