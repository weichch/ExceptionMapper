using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods for building mapping conventions in <see cref="EndpointExceptionHandler"/>.
    /// </summary>
    public static class EndpointExceptionMappingConventionBuildingExtensions
    {
        /// <summary>
        /// Re-executes errored request in an alternative request pipeline when exception type is
        /// <typeparamref name="TException"/>.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="requestDelegate">The request delegate to execute.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static EndpointExceptionHandlerOptions MapToRequestDelegate<TException>(
            this EndpointExceptionHandlerOptions options,
            RequestDelegate requestDelegate,
            IEnumerable<string>? tags = null,
            int order = 0)
        {
            options.Conventions.AddParameterized<DelegateExceptionMappingConvention>(
                tags, order, ReExecuteRequestForExceptionType(requestDelegate));

            return options;

            static EndpointExceptionMappingDelegate ReExecuteRequestForExceptionType(
                RequestDelegate requestDelegate)
            {
                return async (context, httpContext) =>
                {
                    if (!(context.Exception is TException))
                    {
                        return;
                    }

                    await requestDelegate(httpContext);
                    if (!context.Result.IsHandled)
                    {
                        context.Result = ExceptionHandlingResult.Handled();
                    }
                };
            }
        }

        /// <summary>
        /// Maps an exception of <typeparamref name="TException"/> to an HTTP response with specified status code.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="statusCode">The result status code.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static EndpointExceptionHandlerOptions MapToStatusCode<TException>(
            this EndpointExceptionHandlerOptions options,
            int statusCode,
            IEnumerable<string>? tags = null,
            int order = 0)
            => options.MapToRequestDelegate<TException>(
                httpContext =>
                {
                    httpContext.Response.StatusCode = statusCode;
                    return Task.CompletedTask;
                }, tags, order);

        /// <summary>
        /// Re-executes the request in an alternative request pipeline.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="path">The path at which the request is re-executed.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static EndpointExceptionHandlerOptions MapToPath<TException>(
            this EndpointExceptionHandlerOptions options,
            PathString path,
            IEnumerable<string>? tags = null,
            int order = 0)
        {
            return options.MapToRequestDelegate<TException>(httpContext =>
                ReExecuteRequestAtPath(httpContext, path), tags, order);

            static async Task ReExecuteRequestAtPath(HttpContext httpContext, PathString newPath)
            {
                var feature = httpContext.Features.Get<IExceptionMappingFeature>();
                httpContext.Request.Path = newPath;
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
