using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using RabbitSharp.Diagnostics.AspNetCore;
using RabbitSharp.Diagnostics.AspNetCore.Formatting;
using RabbitSharp.Diagnostics.AspNetCore.Internals;

namespace RabbitSharp.Diagnostics.Builder
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
                    var exceptionHandlingContext = httpContext.Features.Get<IExceptionMappingContextFeature>().Context!;
                    exceptionHandlingContext.Result = ExceptionHandlingResult.Rethrow(exceptionHandlingContext.Exception);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RequestDelegate ToNamedExceptionHandler(string name)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RequestDelegate ToProblemDetails(
            Func<ProblemDetailsCreationContext, ProblemDetails> problemFactory)
        {
            return httpContext => WriteProblemDetails(httpContext, problemFactory);

            static Task WriteProblemDetails(
                HttpContext httpContext,
                Func<ProblemDetailsCreationContext, ProblemDetails> problemFactory)
            {
                var exceptionHandlingContext = httpContext.Features.Get<IExceptionMappingContextFeature>().Context!;
                var creationContext = new ProblemDetailsCreationContext(exceptionHandlingContext, httpContext);
                var problem = problemFactory(creationContext);

                // Create problem response writer
                var problemResponseWriter = httpContext.RequestServices
                    .GetRequiredService<IProblemResponseWriterFactory>()
                    .Create(httpContext);

                var writerContext = new ProblemResponseWriterContext(httpContext, problem);
                var feature = httpContext.Features.Get<IExceptionMappingFeature>();
                var endpoint = feature.Endpoint!;
                var actionDescriptor = endpoint.Metadata.GetMetadata<ActionDescriptor>();

                if (actionDescriptor != null)
                {
                    writerContext.ActionContext = new ActionContext(
                        httpContext,
                        new RouteData(feature.RouteValues ?? new RouteValueDictionary()),
                        actionDescriptor);
                }

                return problemResponseWriter.WriteAsync(writerContext);
            }
        }
    }
}
