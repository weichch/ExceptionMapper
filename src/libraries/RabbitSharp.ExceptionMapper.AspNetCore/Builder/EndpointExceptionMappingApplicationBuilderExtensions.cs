using System;
using System.Linq;
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
                Mapper = app.ApplicationServices.GetRequiredService<IExceptionMapper>(),
                Schemes = {EndpointExceptionMappingDefaults.EndpointScheme}
            });

        /// <summary>
        /// Adds exception mapping middleware to re-execute request in an alternative request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="mapper">The exception mapper to use.</param>
        /// <param name="schemes">The schemes to run.</param>
        public static IApplicationBuilder UseEndpointExceptionMapping(
            this IApplicationBuilder app,
            IExceptionMapper? mapper,
            params string[] schemes)
        {
            if (schemes == null)
            {
                throw new ArgumentNullException(nameof(schemes));
            }

            var options = new EndpointExceptionMappingOptions
            {
                Mapper = mapper
            };

            if (!schemes.Any())
            {
                options.Schemes.Add(EndpointExceptionMappingDefaults.EndpointScheme);
            }
            else
            {
                foreach (var scheme in schemes)
                {
                    options.Schemes.Add(scheme);
                }
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
