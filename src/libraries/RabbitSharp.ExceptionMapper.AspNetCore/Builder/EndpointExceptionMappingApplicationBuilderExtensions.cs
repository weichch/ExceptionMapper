using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitSharp.Diagnostics;
using RabbitSharp.Diagnostics.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Provides extensions to <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class EndpointExceptionMappingApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds exception mapping middleware to re-execute request in an alternative request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        public static IApplicationBuilder UseEndpointExceptionMapping(
            this IApplicationBuilder app) =>
            app.UseEndpointExceptionMapping(new EndpointExceptionMappingOptions
            {
                Mapper = app.ApplicationServices.GetRequiredService<IExceptionMapper>()
            });

        /// <summary>
        /// Adds exception mapping middleware to re-execute request in an alternative request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="mapper">The exception mapper to use.</param>
        /// <param name="additionalSchemes">The additional schemes to run.</param>
        public static IApplicationBuilder UseEndpointExceptionMapping(
            this IApplicationBuilder app,
            IExceptionMapper? mapper,
            params string[] additionalSchemes)
        {
            if (additionalSchemes == null)
            {
                throw new ArgumentNullException(nameof(additionalSchemes));
            }

            var options = new EndpointExceptionMappingOptions
            {
                Mapper = mapper
            };

            foreach (var scheme in additionalSchemes)
            {
                options.Schemes.Add(scheme);
            }

            return app.UseEndpointExceptionMapping(options);
        }

        /// <summary>
        /// Adds exception mapping middleware to re-execute request in an alternative request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="options">The settings for middleware.</param>
        public static IApplicationBuilder UseEndpointExceptionMapping(
            this IApplicationBuilder app,
            EndpointExceptionMappingOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            app.UseMiddleware<EndpointExceptionMappingMiddleware>(
                Options.Create(options),
                app.New());

            return app;
        }
    }
}
