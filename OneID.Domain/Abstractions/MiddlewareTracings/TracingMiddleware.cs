using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace OneID.Domain.Abstractions.MiddlewareTracings
{
    public class TracingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ActivitySource Source = new("oneid.auto");

        public TracingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;
            var spanName = $"{method} {path}";

            var kind = Activity.Current == null ? ActivityKind.Server : ActivityKind.Internal;

            using var activity = Source.StartActivity(spanName, kind);

            activity?.SetTag("http.method", method);
            activity?.SetTag("http.route", path);
            activity?.SetTag("request.host", context.Request.Host.ToString());

            try
            {
                await _next(context);
                activity?.SetTag("http.status_code", context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                activity?.SetTag("erro", true);
                activity?.SetTag("exception.message", ex.Message);
                activity?.SetTag("exception.stacktrace", ex.StackTrace);
                throw;
            }
        }

    }

}
