﻿using System;
using Microsoft.AspNetCore.Http;
using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods to <see cref="IEndpointExceptionMappingConventionBuilder"/>
    /// and implementer types.
    /// </summary>
    public static class EndpointExceptionMappingConventionBuilderExtensions
    {
        /// <summary>
        /// Maps exception to a request handler.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="requestHandler">The request handler.</param>
        public static TBuilder ToRequestHandler<TBuilder>(
            this TBuilder builder,
            RequestDelegate requestHandler)
            where TBuilder : IEndpointExceptionMappingConventionBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (requestHandler == null)
            {
                throw new ArgumentNullException(nameof(requestHandler));
            }

            builder.MappingDelegate = async (context, httpContext) =>
            {
                await requestHandler(httpContext);
                if (!context.Result.IsHandled)
                {
                    context.Result = ExceptionHandlingResult.Handled();
                }
            };

            return builder;
        }

        /// <summary>
        /// Maps exception to an HTTP status code.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="statusCode">The response status code.</param>
        public static TBuilder ToStatusCode<TBuilder>(
            this TBuilder builder,
            int statusCode)
            where TBuilder : IEndpointExceptionMappingConventionBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.ToRequestHandler(
                EndpointExceptionMappingFunctions.ToStatusCode(statusCode));
        }

        /// <summary>
        /// Maps exception to another endpoint.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="routeValues">The route values.</param>
        public static TBuilder ToEndpoint<TBuilder>(
            this TBuilder builder,
            string pattern,
            object? routeValues = null)
            where TBuilder : IEndpointExceptionMappingConventionBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            return builder.ToRequestHandler(
                EndpointExceptionMappingFunctions.ToEndpoint(pattern, routeValues));
        }
    }
}
