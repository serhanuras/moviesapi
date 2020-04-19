
using Microsoft.AspNetCore.Builder;
using MoviesAPi.Middlewares;

namespace MoviesAPi.Extensions
{

    public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
}