using System;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Represents settings for <see cref="IExceptionMapper"/>.
    /// </summary>
    public class ExceptionMapperOptions
    {
        private ExceptionMappingDelegate _fallbackHandler;

        /// <summary>
        /// Creates an instance of the settings.
        /// </summary>
        public ExceptionMapperOptions()
        {
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
    }
}
