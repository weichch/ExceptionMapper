using System;
using System.Collections.Generic;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Represents settings for exception mapping services.
    /// </summary>
    public class ExceptionMappingServicesOptions
    {
        private ExceptionMappingDelegate _fallbackHandler;

        /// <summary>
        /// Creates an instance of the options.
        /// </summary>
        public ExceptionMappingServicesOptions()
        {
            Schemes = new Dictionary<string, ExceptionMappingSchemeRegistration>(StringComparer.Ordinal);

            // By default, unhandled exception is rethrown
            _fallbackHandler = ctx =>
            {
                ctx.Result = ExceptionHandlingResult.Rethrow(ctx.Exception);
                return default;
            };
        }

        /// <summary>
        /// Gets or sets a fallback exception handler which is invoked when no exception scheme
        /// has successfully handled the exception.
        /// </summary>
        public ExceptionMappingDelegate FallbackExceptionHandler
        {
            get => _fallbackHandler;
            set => _fallbackHandler = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Returns all registered schemes.
        /// </summary>
        public IDictionary<string, ExceptionMappingSchemeRegistration> Schemes { get; }

        /// <summary>
        /// Registers a scheme.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        public void AddScheme(ExceptionMappingSchemeRegistration scheme)
        {
            if (Schemes.ContainsKey(scheme.Name))
            {
                throw new InvalidOperationException(
                    "An exception mapping scheme with the specified name has already been registered.");
            }

            Schemes.Add(scheme.Name, scheme);
        }
    }
}
