using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Implements <see cref="IExceptionMappingSchemeProvider"/>.
    /// </summary>
    class ExceptionMappingSchemeProvider : IExceptionMappingSchemeProvider
    {
        private readonly ExceptionMappingServicesOptions _mappingOptions;

        public ExceptionMappingSchemeProvider(IOptions<ExceptionMappingServicesOptions> mappingOptions)
        {
            _mappingOptions = mappingOptions.Value;
        }

        public IEnumerable<ExceptionMappingSchemeRegistration> GetSchemes()
        {
            return _mappingOptions.Schemes.Values;
        }

        public ExceptionMappingSchemeRegistration? GetScheme(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _mappingOptions.Schemes.TryGetValue(name, out var result);
            return result;
        }
    }
}
