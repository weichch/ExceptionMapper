using System;
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
    public static class EndpointExceptionMappingBuilder
    {
        /// <summary>
        /// Maps exception of <typeparamref name="TException"/> to another exception mapping.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="another">The mapping to run when exception type is <typeparamref name="TException"/>.</param>
        public static EndpointExceptionMappingDelegate MapExceptionType<TException>(
            EndpointExceptionMappingDelegate another)
        {
            if (another == null)
            {
                throw new ArgumentNullException(nameof(another));
            }

            return (context, httpContext) =>
            {
                if (!(context.Exception is TException))
                {
                    return Task.CompletedTask;
                }

                return another(context, httpContext);
            };
        }

        /// <summary>
        /// Maps exception of <typeparamref name="TException"/> to a request pipeline.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="pipeline">The request pipeline.</param>
        public static EndpointExceptionMappingDelegate MapToPipeline<TException>(
            RequestDelegate pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }

            return MapExceptionType<TException>(async (context, httpContext) =>
            {
                await pipeline(httpContext);
                if (!context.Result.IsHandled)
                {
                    context.Result = ExceptionHandlingResult.Handled();
                }
            });
        }

        /// <summary>
        /// Maps exception of <typeparamref name="TException"/> to an HTTP status code.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="statusCode">The response status code.</param>
        public static EndpointExceptionMappingDelegate MapToStatusCode<TException>(
            int statusCode)
        {
            return MapToPipeline<TException>(httpContext =>
            {
                httpContext.Response.StatusCode = statusCode;
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Maps exception of <typeparamref name="TException"/> to another endpoint at specified path.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="routeValues">The route values.</param>
        public static EndpointExceptionMappingDelegate MapToPath<TException>(
            string pattern,
            object? routeValues = null)
        {
            return MapToPipeline<TException>(httpContext =>
                RewriteRequestToPath(httpContext, pattern, routeValues));

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
