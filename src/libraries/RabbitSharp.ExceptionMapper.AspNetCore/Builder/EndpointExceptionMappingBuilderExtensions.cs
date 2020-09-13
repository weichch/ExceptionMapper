using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitSharp.Diagnostics.AspNetCore;
using RabbitSharp.Diagnostics.AspNetCore.Formatting;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods to <see cref="ExceptionMappingBuilder"/> for ASP.NET Core application.
    /// </summary>
    public static class EndpointExceptionMappingBuilderExtensions
    {
        /// <summary>
        /// Adds an exception mapping scheme which maps exception to HTTP response by convention.
        /// </summary>
        /// <param name="builder">The exception mapping builder.</param>
        public static ExceptionMappingBuilder AddEndpointResponse(this ExceptionMappingBuilder builder)
            => builder.AddEndpointResponse(EndpointExceptionMappingDefaults.EndpointScheme, _ => { });

        /// <summary>
        /// Adds an exception mapping scheme which maps exception to HTTP response by convention.
        /// </summary>
        /// <param name="builder">The exception mapping builder.</param>
        /// <param name="name">The name of the scheme.</param>
        public static ExceptionMappingBuilder AddEndpointResponse(
            this ExceptionMappingBuilder builder,
            string name)
            => builder.AddEndpointResponse(name, _ => { });

        /// <summary>
        /// Adds an exception mapping scheme which maps exception to HTTP response by convention.
        /// </summary>
        /// <param name="builder">The exception mapping builder.</param>
        /// <param name="configure">The action to configure the endpoint exception mapping scheme.</param>
        public static ExceptionMappingBuilder AddEndpointResponse(
            this ExceptionMappingBuilder builder,
            Action<EndpointExceptionHandlerOptions> configure)
            => builder.AddEndpointResponse(EndpointExceptionMappingDefaults.EndpointScheme, configure);

        /// <summary>
        /// Adds an exception mapping scheme which maps exception to HTTP response by convention.
        /// </summary>
        /// <param name="builder">The exception mapping builder.</param>
        /// <param name="name">The name of the scheme.</param>
        /// <param name="configure">The action to configure the endpoint exception mapping scheme.</param>
        public static ExceptionMappingBuilder AddEndpointResponse(
            this ExceptionMappingBuilder builder,
            string name,
            Action<EndpointExceptionHandlerOptions> configure)
        {
            builder.Services.TryAddSingleton<IHttpContextFinder, HttpContextFinder>();
            builder.Services.TryAddSingleton<IRoutePatternFormatter, RoutePatternFormatter>();
            builder.Services.TryAddSingleton<IProblemResponseWriterFactory, ProblemResponseWriterFactory>();

            builder.AddScheme<EndpointExceptionHandlerOptions, IEndpointExceptionMappingConvention,
                EndpointExceptionHandler>(name, configure);

            return builder;
        }
    }
}
