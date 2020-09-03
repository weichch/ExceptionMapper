using System;
using System.Collections.Generic;
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
        /// Maps exception to an exception mapping function.
        /// </summary>
        /// <typeparam name="TOptions">The type of options.</typeparam>
        /// <param name="options">The options.</param>
        /// <param name="mappingDelegate">The exception mapping function to execute.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static TOptions Map<TOptions>(
            this TOptions options,
            EndpointExceptionMappingDelegate mappingDelegate,
            IEnumerable<string>? tags = null,
            int order = 0)
            where TOptions : EndpointExceptionHandlerOptions
        {
            if (mappingDelegate == null)
            {
                throw new ArgumentNullException(nameof(mappingDelegate));
            }

            options.Conventions.AddParameterized<DelegateExceptionMappingConvention>(
                tags, order, mappingDelegate);

            return options;
        }

        /// <summary>
        /// Maps exception to an exception mapping function when exception handling context
        /// meets criteria.
        /// </summary>
        /// <typeparam name="TOptions">The type of options.</typeparam>
        /// <param name="options">The options.</param>
        /// <param name="predicate">The predicate function.</param>
        /// <param name="mappingDelegate">The exception mapping function to execute.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static TOptions MapWhen<TOptions>(
            this TOptions options,
            Func<ExceptionHandlingContext, HttpContext, bool> predicate,
            EndpointExceptionMappingDelegate mappingDelegate,
            IEnumerable<string>? tags = null,
            int order = 0)
            where TOptions : EndpointExceptionHandlerOptions
            => options.Map(EndpointExceptionMappingBuilder.MapWhen(predicate, mappingDelegate), tags, order);

        /// <summary>
        /// Maps exception to a request pipeline when exception handling context
        /// meets criteria.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="predicate">The predicate function.</param>
        /// <param name="requestDelegate">The request delegate to execute.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static TOptions MapToPipelineWhen<TOptions>(
            this TOptions options,
            Func<ExceptionHandlingContext, HttpContext, bool> predicate,
            RequestDelegate requestDelegate,
            IEnumerable<string>? tags = null,
            int order = 0)
            where TOptions : EndpointExceptionHandlerOptions
            => options.MapWhen(predicate, EndpointExceptionMappingBuilder.MapToPipeline(requestDelegate), tags, order);

        /// <summary>
        /// Maps exception to an HTTP status code when exception handling context
        /// meets criteria.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="predicate">The predicate function.</param>
        /// <param name="statusCode">The result status code.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static TOptions MapToStatusCodeWhen<TOptions>(
            this TOptions options,
            Func<ExceptionHandlingContext, HttpContext, bool> predicate,
            int statusCode,
            IEnumerable<string>? tags = null,
            int order = 0)
            where TOptions : EndpointExceptionHandlerOptions
            => options.MapWhen(predicate, EndpointExceptionMappingBuilder.MapToStatusCode(statusCode), tags, order);

        /// <summary>
        /// Maps exception to another endpoint at specified path when exception handling context
        /// meets criteria.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="predicate">The predicate function.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static TOptions MapToPathWhen<TOptions>(
            this TOptions options,
            Func<ExceptionHandlingContext, HttpContext, bool> predicate,
            string pattern,
            object? routeValues = null,
            IEnumerable<string>? tags = null,
            int order = 0)
            where TOptions : EndpointExceptionHandlerOptions
            => options.MapWhen(predicate, EndpointExceptionMappingBuilder.MapToPath(pattern, routeValues),
                tags, order);

        /// <summary>
        /// Maps exception of <typeparamref name="TException"/> to a request pipeline.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="mappingDelegate">The exception mapping function to execute.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static EndpointExceptionHandlerOptions MapType<TException>(
            this EndpointExceptionHandlerOptions options,
            EndpointExceptionMappingDelegate mappingDelegate,
            IEnumerable<string>? tags = null,
            int order = 0)
            => options.Map(EndpointExceptionMappingBuilder.MapExceptionType<TException>(mappingDelegate), tags, order);

        /// <summary>
        /// Maps exception of <typeparamref name="TException"/> to a request pipeline.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="requestDelegate">The request delegate to execute.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static EndpointExceptionHandlerOptions MapToPipeline<TException>(
            this EndpointExceptionHandlerOptions options,
            RequestDelegate requestDelegate,
            IEnumerable<string>? tags = null,
            int order = 0)
            => options.MapType<TException>(EndpointExceptionMappingBuilder.MapToPipeline(requestDelegate), tags, order);

        /// <summary>
        /// Maps exception of <typeparamref name="TException"/> to an HTTP status code.
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
            => options.MapType<TException>(EndpointExceptionMappingBuilder.MapToStatusCode(statusCode), tags, order);

        /// <summary>
        /// Maps exception of <typeparamref name="TException"/> to another endpoint at specified path.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static EndpointExceptionHandlerOptions MapToPath<TException>(
            this EndpointExceptionHandlerOptions options,
            string pattern,
            object? routeValues = null,
            IEnumerable<string>? tags = null,
            int order = 0)
            => options.MapType<TException>(EndpointExceptionMappingBuilder.MapToPath(pattern, routeValues),
                tags, order);
    }
}
