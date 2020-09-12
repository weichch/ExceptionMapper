using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods to <see cref="EndpointExceptionMappingConventionBuilder"/>
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
            where TBuilder : EndpointExceptionMappingConventionBuilder
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
            where TBuilder : EndpointExceptionMappingConventionBuilder
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
            where TBuilder : EndpointExceptionMappingConventionBuilder
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

        /// <summary>
        /// TODO: Internal for now until implemented. Maps exception to a named exception handler.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The name of the exception handler.</param>
        internal static TBuilder ToNamedExceptionHandler<TBuilder>(
            this TBuilder builder,
            string name)
            where TBuilder : EndpointExceptionMappingConventionBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return builder.ToRequestHandler(
                EndpointExceptionMappingFunctions.ToNamedExceptionHandler(name));
        }

        /// <summary>
        /// TODO: Internal for now until implemented. Maps exception to an instance of <see cref="ProblemDetails"/>
        /// and writes the instance to response.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="problemFactory">The problem details factory.</param>
        internal static TBuilder ToProblemDetails<TBuilder>(
            this TBuilder builder,
            Func<ExceptionHandlingContext, HttpContext, ProblemDetails> problemFactory)
            where TBuilder : EndpointExceptionMappingConventionBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (problemFactory == null)
            {
                throw new ArgumentNullException(nameof(problemFactory));
            }

            return builder.ToRequestHandler(
                EndpointExceptionMappingFunctions.ToProblemDetails(problemFactory));
        }
    }
}
