using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitSharp.Diagnostics;
using RabbitSharp.Diagnostics.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods to <see cref="IServiceCollection"/> for exception mapping.
    /// </summary>
    public static class ExceptionMapperServiceCollectionExtensions
    {
        /// <summary>
        /// Adds exception mapping infrastructure and core services with a set of default options
        /// to the service collection and returns a builder instance for configuring exception
        /// handling by convention.
        /// </summary>
        public static ExceptionMappingBuilder AddExceptionMapping(
            this IServiceCollection serviceCollection)
            => AddExceptionMapping(serviceCollection, _ => { });

        /// <summary>
        /// Adds exception mapping infrastructure and core services to the service collection
        /// and returns a builder instance for configuring exception handling by convention.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configure">The action to configure exception mapping options.</param>
        public static ExceptionMappingBuilder AddExceptionMapping(
            this IServiceCollection serviceCollection,
            Action<ExceptionMappingServicesOptions> configure)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            serviceCollection.TryAddTransient<IExceptionMapper, ExceptionMapper>();
            serviceCollection.TryAddTransient<IExceptionHandlerProvider, ExceptionHandlerProvider>();
            serviceCollection.TryAddTransient<IExceptionMappingConventionProvider,
                ExceptionMappingConventionProvider>();
            serviceCollection.TryAddSingleton<IExceptionMappingSchemeProvider,
                ExceptionMappingSchemeProvider>();
            serviceCollection.Configure(configure);

            return new ExceptionMappingBuilder(serviceCollection);
        }
    }
}
