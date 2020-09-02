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
        /// Maps exception of <typeparamref name="TException"/> to another exception mapping.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="mappingDelegate">The exception mapping function to execute.</param>
        /// <param name="tags">The user-defined tags on the mapping convention.</param>
        /// <param name="order">The order of the mapping convention.</param>
        public static EndpointExceptionHandlerOptions Map<TException>(
            this EndpointExceptionHandlerOptions options,
            EndpointExceptionMappingDelegate mappingDelegate,
            IEnumerable<string>? tags = null,
            int order = 0)
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
            => options.Map<TException>(EndpointExceptionMappingBuilder.MapToPipeline<TException>(
                requestDelegate), tags, order);

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
            => options.Map<TException>(EndpointExceptionMappingBuilder.MapToStatusCode<TException>(
                statusCode), tags, order);

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
            => options.Map<TException>(EndpointExceptionMappingBuilder.MapToPath<TException>(
                pattern, routeValues), tags, order);
    }
}
