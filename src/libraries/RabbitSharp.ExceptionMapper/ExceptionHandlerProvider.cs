using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Implements <see cref="IExceptionHandlerProvider"/>.
    /// </summary>
    class ExceptionHandlerProvider : IExceptionHandlerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Task<IExceptionHandler>> _cache;
        private readonly object _lock = new object();

        public ExceptionHandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cache = new Dictionary<string, Task<IExceptionHandler>>(StringComparer.Ordinal);
        }

        public Task<IExceptionHandler> GetHandlerAsync(ExceptionMappingSchemeRegistration scheme)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            Task<IExceptionHandler>? handlerTask;
            lock (_lock)
            {
                // Happy path: get from cache.
                if (!_cache.TryGetValue(scheme.Name, out handlerTask))
                {
                    // Slow path: create the handler and initialize it
                    // The handler task may complete synchronously here
                    handlerTask = AwaitedCreate(scheme, _serviceProvider);
                    _cache.Add(scheme.Name, handlerTask);
                }
            }

            return handlerTask;

            static async Task<IExceptionHandler> AwaitedCreate(
                ExceptionMappingSchemeRegistration scheme,
                IServiceProvider serviceProvider)
            {
                var handler = scheme.HandlerFactory(serviceProvider);
                await handler.InitializeAsync(scheme.Name);
                return handler;
            }
        }
    }
}
