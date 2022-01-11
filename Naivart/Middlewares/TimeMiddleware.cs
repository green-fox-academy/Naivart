using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Naivart.Services;
using System.Threading.Tasks;

namespace Naivart.Middlewares
{
    public class TimeMiddleware
    {
        private readonly RequestDelegate _next;

        public TimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task<Task> InvokeAsync(HttpContext httpContext, TimeService timeService)
        {
            var kingdomId = httpContext.Request.RouteValues["id"];
            if (kingdomId is not null)
            {
                long.TryParse(kingdomId.ToString(), out long result);
                await timeService.UpdateAllAsync(result);
            }
            return _next(httpContext);
        }
    }

    public static class TimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseTimeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TimeMiddleware>();
        }
    }
}
