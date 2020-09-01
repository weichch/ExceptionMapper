using System;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides methods to build exception handling by convention.
    /// </summary>
    public class ExceptionMappingBuilder
    {
        /// <summary>
        /// Creates an instance of the builder.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        public ExceptionMappingBuilder(IServiceCollection serviceCollection)
        {
            Services = serviceCollection
                       ?? throw new ArgumentNullException(nameof(serviceCollection));
        }

        /// <summary>
        /// Gets the service collection used by the builder.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Registers a strongly-typed exception mapping scheme.
        /// </summary>
        /// <param name="scheme">The scheme to be added.</param>
        public void AddScheme(ExceptionMappingSchemeRegistration scheme)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            Services.Configure<ExceptionMappingServicesOptions>(
                opt => opt.AddScheme(scheme));
        }
    }
}
