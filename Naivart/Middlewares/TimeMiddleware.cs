using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Naivart.Interfaces.ServiceInterfaces;
using Naivart.Services;
using System;
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

        public async Task InvokeAsync(HttpContext httpContext, ITimeService timeService)
        {
            string username = httpContext.User.Identity.Name;
            if (!String.IsNullOrEmpty(username))
            {
                await timeService.UpdateAllAsync(username);
            }
            await _next(httpContext);
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
