using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using RabbitSharp.Diagnostics.AspNetCore.Internals;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Defines exception mapping functions.
    /// </summary>
    static class EndpointExceptionMappingFunctions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RequestDelegate ToStatusCode(int statusCode)
        {
            return httpContext =>
            {
                httpContext.Response.StatusCode = statusCode;
                return Task.CompletedTask;
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RequestDelegate ToEndpoint(
            string pattern,
            object? routeValues = null)
        {
            return httpContext => RewriteRequestToPath(httpContext, pattern, routeValues);

            static async Task RewriteRequestToPath(
                HttpContext httpContext, 
                string pattern, 
                object? routeValues)
            {
                var routePatternFormatter = httpContext.RequestServices.GetRequiredService<IRoutePatternFormatter>();
                var feature = httpContext.Features.Get<IExceptionMappingFeature>();
                
                var path = routePatternFormatter.Format(httpContext, RoutePatternFactory.Parse(pattern), routeValues);
                if (path == null)
                {
                    // Unable to format route pattern, rethrow the exception
                    var context = httpContext.Features.Get<IExceptionMappingContextFeature>().Context!;
                    context.Result = ExceptionHandlingResult.Rethrow(context.Exception);
                    return;
                }

                httpContext.Request.Path = path;
                try
                {
                    await feature.RequestPipeline(httpContext);
                }
                finally
                {
                    httpContext.Request.Path = feature.RequestPath;
                }
            }
        }
    }
}
