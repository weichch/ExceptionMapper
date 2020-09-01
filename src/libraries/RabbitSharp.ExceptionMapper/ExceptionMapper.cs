using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Implements <see cref="IExceptionMapper"/>.
    /// </summary>
    class ExceptionMapper : IExceptionMapper
    {
        private readonly ExceptionMapperOptions _options;
        private readonly IExceptionHandlerProvider _handlerProvider;
        private readonly IExceptionMappingSchemeProvider _schemeProvider;
        private readonly ILogger _logger;

        public ExceptionMapper(
            IOptions<ExceptionMapperOptions> options,
            IExceptionHandlerProvider handlerProvider,
            IExceptionMappingSchemeProvider schemeProvider,
            ILoggerFactory loggerFactory)
        {
            _options = options.Value;
            _handlerProvider = handlerProvider;
            _schemeProvider = schemeProvider;
            _logger = loggerFactory.CreateLogger(typeof(ExceptionMapper));
        }

        public async Task<ExceptionHandlingResult> MapAsync(
            Exception exception,
            ExceptionMappingContext? context)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            context ??= new ExceptionMappingContext();
            try
            {
                foreach (var scheme in _schemeProvider.GetSchemes())
                {
                    if (context.SchemeFilter?.Invoke(scheme) == false)
                    {
                        continue;
                    }

                    var handler = await _handlerProvider.GetHandlerAsync(scheme);
                    var result = await handler.HandleExceptionAsync(exception, context);
                    if (result.IsHandled && result.Handling != ExceptionHandling.Skip)
                    {
                        return result;
                    }
                }

                var fallbackContext = new ExceptionHandlingContext(exception, context);
                await _options.FallbackExceptionHandler(fallbackContext);
                return fallbackContext.Result;
            }
            catch (Exception ex)
            {
                // An unhandled exception occurred while executing the handler
                // This is not expected as handlers should be exception-free. When this happens, log the exception
                // and re-throw the *original* exception
                _logger.LogTrace(ex, "Unhandled exception occurred in handlers.");
            }

            return ExceptionHandlingResult.Rethrow(exception);
        }
    }
}
