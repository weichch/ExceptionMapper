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
        /// Maps exception to an exception mapping function when exception handling context
        /// meets criteria.
        /// </summary>
        /// <param name="predicate">The predicate function.</param>
        /// <param name="mappingDelegate">The mapping to run when criteria met.</param>
        public static EndpointExceptionMappingDelegate MapWhen(
            Func<ExceptionHandlingContext, HttpContext, bool> predicate,
            EndpointExceptionMappingDelegate mappingDelegate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (mappingDelegate == null)
            {
                throw new ArgumentNullException(nameof(mappingDelegate));
            }

            return (context, httpContext) =>
            {
                if (predicate(context, httpContext))
                {
                    return mappingDelegate(context, httpContext);
                }

                return Task.CompletedTask;
            };
        }

        /// <summary>
        /// Maps exception of <typeparamref name="TException"/> to an exception mapping function.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="mappingDelegate">The mapping to run when exception type is <typeparamref name="TException"/>.</param>
        public static EndpointExceptionMappingDelegate MapExceptionType<TException>(
            EndpointExceptionMappingDelegate mappingDelegate)
        {
            if (mappingDelegate == null)
            {
                throw new ArgumentNullException(nameof(mappingDelegate));
            }

            return MapWhen((context, _) => context.Exception is TException, mappingDelegate);
        }

        /// <summary>
        /// Maps exception to a request pipeline.
        /// </summary>
        /// <param name="pipeline">The request pipeline.</param>
        public static EndpointExceptionMappingDelegate MapToPipeline(
            RequestDelegate pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }

            return async (context, httpContext) =>
            {
                await pipeline(httpContext);
                if (!context.Result.IsHandled)
                {
                    context.Result = ExceptionHandlingResult.Handled();
                }
            };
        }

        /// <summary>
        /// Maps exception to an HTTP status code.
        /// </summary>
        /// <param name="statusCode">The response status code.</param>
        public static EndpointExceptionMappingDelegate MapToStatusCode(
            int statusCode)
        {
            return MapToPipeline(httpContext =>
            {
                httpContext.Response.StatusCode = statusCode;
                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Maps exception to another endpoint at specified path.
        /// </summary>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="routeValues">The route values.</param>
        public static EndpointExceptionMappingDelegate MapToPath(
            string pattern,
            object? routeValues = null)
        {
            return MapToPipeline(httpContext =>
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
