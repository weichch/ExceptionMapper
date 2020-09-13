using System;
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
                ApplicationBuilder = app.New()
            });

        /// <summary>
        /// Adds exception mapping middleware to re-execute request in an alternative request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="mapper">The exception mapper to use.</param>
        public static IApplicationBuilder UseEndpointExceptionMapping(
            this IApplicationBuilder app,
            IExceptionMapper mapper)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            var options = new EndpointExceptionMappingOptions
            {
                Mapper = mapper,
                ApplicationBuilder = app.New()
            };

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
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ApplicationBuilder ??= app;
            app.UseMiddleware<EndpointExceptionMappingMiddleware>(
                Options.Create(options));

            return app;
        }
    }
}
