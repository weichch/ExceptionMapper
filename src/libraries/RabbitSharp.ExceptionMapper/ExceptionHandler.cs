using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Provides base implementation for convention-based exception handler type which uses strongly typed settings.
    /// </summary>
    /// <typeparam name="TOptions">The user-defined options type.</typeparam>
    /// <typeparam name="TConventionService">The mapping convention service type.</typeparam>
    public abstract class ExceptionHandler<TOptions, TConventionService> : IExceptionHandler
        where TOptions : ExceptionMappingSchemeOptions, new()
        where TConventionService : class
    {
        private readonly IOptionsMonitor<TOptions> _optionsManager;
        private readonly IExceptionMappingConventionProvider _conventionProvider;
        private TOptions? _optionsValue;
        private ILogger _logger;

        /// <summary>
        /// Initializes the exception handler.
        /// </summary>
        /// <param name="optionsManager">The options manager for retrieving named options.</param>
        /// <param name="conventionProvider">The service to provision mapping convention instances for this scheme.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        protected ExceptionHandler(
            IOptionsMonitor<TOptions> optionsManager,
            IExceptionMappingConventionProvider conventionProvider,
            ILoggerFactory loggerFactory)
        {
            _optionsManager = optionsManager ?? throw new ArgumentNullException(nameof(optionsManager));
            _conventionProvider = conventionProvider ?? throw new ArgumentNullException(nameof(conventionProvider));

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger(GetType());
            Conventions = Array.Empty<TConventionService>();
        }

        /// <summary>
        /// Gets initialized options for current exception mapping scheme.
        /// </summary>
        public TOptions Options
        {
            get
            {
                if (_optionsValue == null)
                {
                    throw new InvalidOperationException(
                        "The exception handler has not been initialized yet. Call InitializeAsync method.");
                }

                return _optionsValue;
            }
            private set => _optionsValue = value;
        }

        /// <summary>
        /// Gets a collection of mapping conventions registered to the scheme.
        /// </summary>
        public IReadOnlyCollection<TConventionService> Conventions { get; private set; }

        /// <summary>
        /// Gets the logger for this exception handler.
        /// </summary>
        public ILogger Logger
        {
            get => _logger;
            set => _logger = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Initializes the exception handler for a specified scheme.
        /// </summary>
        /// <param name="name">The name of the scheme.</param>
        public virtual async ValueTask InitializeAsync(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Options = _optionsManager.Get(name);
            await InitializeConventionsAsync(name);
        }

        /// <summary>
        /// Initializes conventions registered to the handler.
        /// </summary>
        /// <param name="name">The name of the scheme.</param>
        protected virtual ValueTask InitializeConventionsAsync(string name)
        {
            Conventions = _conventionProvider.GetConventions<TOptions>(name)
                .OfType<TConventionService>()
                .ToList();
            return default;
        }

        /// <summary>
        /// Handles an exception by convention.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="mappingContext">The data shared by all components within current mapping.</param>
        public ValueTask<ExceptionHandlingResult> HandleExceptionAsync(
            Exception exception,
            ExceptionMappingContext mappingContext)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (mappingContext == null)
            {
                throw new ArgumentNullException(nameof(mappingContext));
            }

            Logger.LogTrace("Running exception handler '{Name}' of type '{HandlerType}'.",
                Options.SchemeName, GetType().FullName);

            var handlingContext = new ExceptionHandlingContext(exception, mappingContext);
            return HandleExceptionAsync(handlingContext);
        }

        /// <summary>
        /// In derived classes, handle exception by convention.
        /// </summary>
        /// <param name="context">The exception handling data context.</param>
        protected abstract ValueTask<ExceptionHandlingResult> HandleExceptionAsync(
            ExceptionHandlingContext context);
    }
}
