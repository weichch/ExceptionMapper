using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Represents a middleware for exception mapping.
    /// </summary>
    class EndpointExceptionMappingMiddleware
    {
        private readonly EndpointExceptionMappingOptions _options;
        private readonly RequestDelegate _exceptionMappingPipeline;
        private readonly RequestDelegate _next;

        public EndpointExceptionMappingMiddleware(
            RequestDelegate next,
            IOptions<EndpointExceptionMappingOptions> options,
            IApplicationBuilder app)
        {
            _options = options.Value;
            // Save the original next for exception mapping
            _next = next;
            _exceptionMappingPipeline = BuildExceptionMappingPipeline(this, app, next);
        }

        public Task Invoke(HttpContext httpContext)
            => _exceptionMappingPipeline(httpContext);

        /// <summary>
        /// Injects exception mapper into exception handler middleware and wraps the current request
        /// pipeline into the new pipeline.
        /// </summary>
        static RequestDelegate BuildExceptionMappingPipeline(
            EndpointExceptionMappingMiddleware middleware,
            IApplicationBuilder app,
            RequestDelegate next)
        {
            var exceptionHandlerOptions = new ExceptionHandlerOptions
            {
                ExceptionHandler = middleware.OnExceptionHandler
            };

            return
                // Wrap everything with exception handler middleware with exception mapping injected
                app.UseExceptionHandler(exceptionHandlerOptions)
                    // Use a middleware to capture exception mapping context when exception occurred
                    // This is needed because exception handler middleware clears required endpoint
                    // information before calling the handler delegate
                    .Use(middleware.CaptureExceptionMappingContext)
                    // Call original request pipeline in wrapped pipeline
                    .Use(_ => next)
                    .Build();
        }

        private Task CaptureExceptionMappingContext(HttpContext httpContext, Func<Task> next)
        {
            ExceptionDispatchInfo? edi = null;
            try
            {
                var task = next();
                if (!task.IsCompletedSuccessfully)
                {
                    return Awaited(this, task, httpContext);
                }
            }
            catch (Exception ex)
            {
                edi = ExceptionDispatchInfo.Capture(ex);
            }

            if (edi != null)
            {
                PrepareExceptionMappingContext(this, httpContext);
                edi.Throw();
            }

            return Task.CompletedTask;

            static async Task Awaited(EndpointExceptionMappingMiddleware middleware, Task task, HttpContext httpContext)
            {
                ExceptionDispatchInfo? edi2 = null;
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    edi2 = ExceptionDispatchInfo.Capture(ex);
                }

                if (edi2 != null)
                {
                    PrepareExceptionMappingContext(middleware, httpContext);
                    edi2.Throw();
                }
            }
        }

        private Task OnExceptionHandler(HttpContext httpContext)
        {
            var exception = httpContext.Features.Get<IExceptionHandlerFeature>().Error;
            if (exception == null)
            {
                return Task.CompletedTask;
            }

            return MapExceptionToResponse(this, exception, httpContext);
        }

        private static async Task MapExceptionToResponse(
            EndpointExceptionMappingMiddleware middleware,
            Exception exception,
            HttpContext httpContext)
        {
            var options = middleware._options;
            // Use cached mapper or activate a mapper in request scope
            var mapper = options.Mapper ?? httpContext.RequestServices.GetRequiredService<IExceptionMapper>();

            var mappingContext = new ExceptionMappingContext();
            mappingContext.SetHttpContext(httpContext);
            mappingContext.SchemeFilter = scheme => options.Schemes.Contains(scheme.Name);

            var mappingResult = await mapper.MapAsync(exception, mappingContext);
            if (!mappingResult.IsHandledSuccessfully || mappingResult.Handling == ExceptionHandling.Return)
            {
                // Can't return object from this middleware
                // If the result was handled as returning object,
                // we should rethrow the exception instead.
                ExceptionHandlingResult.Rethrow(exception).GetResult();
            }
            else
            {
                mappingResult.GetResult();
            }
        }

        private static void PrepareExceptionMappingContext(EndpointExceptionMappingMiddleware middleware, HttpContext httpContext)
        {
            var exceptionMappingFeature = new ExceptionMappingFeature
            {
                RequestPipeline = middleware._next,
                RequestPath = httpContext.Request.Path,
                RequestPathBase = httpContext.Request.PathBase
            };
            var originalEndpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            var routeValues = httpContext.Features.Get<IRouteValuesFeature>()?.RouteValues;

            exceptionMappingFeature.Endpoint = originalEndpoint;
            exceptionMappingFeature.RouteValues = routeValues == null
                ? null
                : RouteValueDictionary.FromArray(routeValues.ToArray()!);

            httpContext.Features.Set<IExceptionMappingFeature>(exceptionMappingFeature);
            httpContext.Features.Set<IExceptionMappingContextFeature>(exceptionMappingFeature);
        }
    }
}
