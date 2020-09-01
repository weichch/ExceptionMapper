using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Implements <see cref="IExceptionMappingConventionProvider"/>.
    /// </summary>
    class ExceptionMappingConventionProvider : IExceptionMappingConventionProvider
    {
        private readonly IExceptionMappingSchemeProvider _schemeProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<object, ExceptionMappingConventionRegistration> _registrationLookup;
        private readonly Dictionary<string, object[]> _cache;

        public ExceptionMappingConventionProvider(
            IExceptionMappingSchemeProvider schemeProvider,
            IServiceProvider serviceProvider)
        {
            _schemeProvider = schemeProvider;
            _serviceProvider = serviceProvider;
            _cache = new Dictionary<string, object[]>(StringComparer.Ordinal);
            _registrationLookup = new Dictionary<object, ExceptionMappingConventionRegistration>();
        }

        public IEnumerable<object> GetConventions<TOptions>(string scheme)
            where TOptions : ExceptionMappingSchemeOptions
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            if (_cache.TryGetValue(scheme, out var cachedResults))
            {
                return cachedResults;
            }

            var schemeRegistration = _schemeProvider.GetScheme(scheme);
            cachedResults = schemeRegistration == null
                ? Array.Empty<object>()
                : GetConventionsSlow<TOptions>(scheme).ToArray();

            _cache.Add(scheme, cachedResults);
            return cachedResults;
        }

        /// <summary>
        /// Creates conventions from options and sort created by order property.
        /// </summary>
        private IEnumerable<object> GetConventionsSlow<TOptions>(string scheme)
            where TOptions : ExceptionMappingSchemeOptions
        {
            var optionsManager = _serviceProvider.GetRequiredService<IOptionsMonitor<TOptions>>();

            foreach (var registration in optionsManager.Get(scheme).Conventions
                .OrderBy(convention => convention.Order))
            {
                var convention = registration.ConventionFactory(_serviceProvider);
                if (convention == null)
                {
                    continue;
                }

                if (!_registrationLookup.ContainsKey(convention))
                {
                    _registrationLookup.Add(convention, registration);
                }

                yield return convention;
            }
        }

        public ExceptionMappingConventionRegistration? GetConventionRegistration(object convention)
        {
            if (convention == null)
            {
                throw new ArgumentNullException(nameof(convention));
            }

            _registrationLookup.TryGetValue(convention, out var result);
            return result;
        }
    }
}
