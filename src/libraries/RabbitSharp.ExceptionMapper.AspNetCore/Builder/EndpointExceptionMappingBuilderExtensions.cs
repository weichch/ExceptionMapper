using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods to <see cref="ExceptionMappingBuilder"/> for ASP.NET Core application.
    /// </summary>
    public static class EndpointExceptionMappingBuilderExtensions
    {
        /// <summary>
        /// Adds an exception mapping scheme which handles exception from endpoint in current ASP.NET Core application
        /// and re-executes the errored request in an alternative pipeline by convention.
        /// </summary>
        /// <param name="builder">The exception mapping builder.</param>
        public static ExceptionMappingBuilder AddEndpointResponse(this ExceptionMappingBuilder builder)
            => builder.AddEndpointResponse(_ => { });

        /// <summary>
        /// Adds an exception mapping scheme which handles exception from endpoint in current ASP.NET Core application
        /// and re-executes the errored request in an alternative pipeline by convention.
        /// </summary>
        /// <param name="builder">The exception mapping builder.</param>
        /// <param name="configure">The action to configure the endpoint exception mapping scheme.</param>
        public static ExceptionMappingBuilder AddEndpointResponse(
            this ExceptionMappingBuilder builder,
            Action<EndpointExceptionHandlerOptions> configure)
        {
            builder.Services.TryAddSingleton<IHttpContextFinder, HttpContextFinder>();
            builder.AddScheme<EndpointExceptionHandlerOptions, IEndpointExceptionMappingConvention,
                EndpointExceptionHandler>(EndpointExceptionMappingDefaults.EndpointScheme, configure);

            return builder;
        }
    }
}
