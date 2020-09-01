using System;
using System.Collections.Generic;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// An internal options type for exception mapping services.
    /// </summary>
    class ExceptionMappingServicesOptions
    {
        /// <summary>
        /// Creates an instance of the options.
        /// </summary>
        public ExceptionMappingServicesOptions()
        {
            SchemeRegistry = new Dictionary<string, ExceptionMappingSchemeRegistration>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Returns all registered schemes.
        /// </summary>
        public IDictionary<string, ExceptionMappingSchemeRegistration> SchemeRegistry { get; }

        /// <summary>
        /// Registers a scheme.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        public void AddScheme(ExceptionMappingSchemeRegistration scheme)
        {
            if (SchemeRegistry.ContainsKey(scheme.Name))
            {
                throw new InvalidOperationException(
                    "An exception mapping scheme with the specified name has already been registered.");
            }

            SchemeRegistry.Add(scheme.Name, scheme);
        }
    }
}
