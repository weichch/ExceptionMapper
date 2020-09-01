using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitSharp.Diagnostics.AspNetCore.Filters;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Represents an exception mapping scheme which handles exception from an endpoint in ASP.NET Core application
    /// using registered <see cref="IEndpointExceptionMappingConvention"/> objects.
    /// </summary>
    public class EndpointExceptionHandler : ExceptionHandler<EndpointExceptionHandlerOptions, IEndpointExceptionMappingConvention>
    {
        private readonly IExceptionMappingConventionProvider _conventionProvider;
        private readonly IHttpContextFinder _httpContextFinder;

        /// <summary>
        /// Creates an instance of the exception handler.
        /// </summary>
        /// <param name="optionsManager">The options manager.</param>
        /// <param name="conventionProvider">The convention provider.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="httpContextFinder">The HTTP context finder.</param>
        public EndpointExceptionHandler(
            IOptionsMonitor<EndpointExceptionHandlerOptions> optionsManager,
            IExceptionMappingConventionProvider conventionProvider,
            ILoggerFactory loggerFactory,
            IHttpContextFinder httpContextFinder
        ) : base(optionsManager, conventionProvider, loggerFactory)
        {
            _conventionProvider = conventionProvider;
            _httpContextFinder = httpContextFinder ?? throw new ArgumentNullException(nameof(httpContextFinder));
        }

        /// <summary>
        /// Handles exception by convention.
        /// </summary>
        /// <param name="context">The exception handling context.</param>
        protected override async ValueTask<ExceptionHandlingResult> HandleExceptionAsync(
            ExceptionHandlingContext context)
        {
            var httpContext = _httpContextFinder.GetHttpContext(context.ParentContext);
            if (httpContext == null)
            {
                Logger.LogTrace(
                    "No HTTP context found. ASP.NET Core endpoint exception mapping scheme requires HTTP context. " +
                    "Try provide the context in one of the data context or add 'IHttpContextAccessor' to your application. " +
                    "Alternatively, provide your own 'IHttpContextFinder' implementation.");
                return default;
            }

            // Set exception handling context to feature so that it is visible to
            // all request delegates.
            httpContext.Features.Get<IExceptionMappingContextFeature>().Context = context;
            try
            {
                await HandleByMetadata(context, httpContext);
                if (context.Result.IsHandled)
                {
                    return context.Result;
                }

                await HandleByConventions(context, httpContext);
                if (context.Result.IsHandled)
                {
                    return context.Result;
                }

                await HandleByFallbackRequestDelegate(context, httpContext);
                return context.Result;
            }
            finally
            {
                httpContext.Features.Get<IExceptionMappingContextFeature>().Context = null;
            }
        }

        /// <summary>
        /// Handles exception by metadata on endpoint.
        /// </summary>
        private ValueTask HandleByMetadata(ExceptionHandlingContext context, HttpContext httpContext)
        {
            var endpoint = httpContext.Features.Get<IExceptionMappingFeature>().Endpoint;
            if (endpoint == null)
            {
                // We only handle exception from an endpoint as we need to read
                // user-defined configuration from endpoint
                return SkipHandler(context, Logger);
            }

            // Configure as per metadata
            var metadata = endpoint.Metadata.GetMetadata<MapExceptionAttribute>();
            if (metadata == null)
            {
                if (!Options.ImplicitExceptionMapping)
                {
                    return SkipHandler(context, Logger);
                }

                metadata = new MapExceptionAttribute();
            }
            else if (metadata.Exclude)
            {
                return SkipHandler(context, Logger);
            }

            // Filter convention by metadata
            context.ConventionFilter = convention =>
                metadata.Tags.Count == 0
                || convention.Tags.Overlaps(metadata.Tags);

            return default;

            static ValueTask SkipHandler(ExceptionHandlingContext context, ILogger logger)
            {
                logger.LogTrace("No endpoint or no required metadata. This handler will be skipped.");
                context.Result = ExceptionHandlingResult.Skip();
                return default;
            }
        }

        /// <summary>
        /// Handles exception by registered conventions.
        /// </summary>
        private async ValueTask HandleByConventions(ExceptionHandlingContext context, HttpContext httpContext)
        {
            foreach (var convention in Conventions)
            {
                var registration = _conventionProvider.GetConventionRegistration(convention);
                if (registration == null)
                {
                    continue;
                }

                if (context.ConventionFilter?.Invoke(registration) == false)
                {
                    continue;
                }

                await convention.ExecuteAsync(context, httpContext);
                if (context.Result.IsHandled)
                {
                    if (context.Result.Handling == ExceptionHandling.Return)
                    {
                        Logger.LogTrace("Exception mapping convention returned an uninterpretable result.");
                        // Return result is not supported by this handler
                        // Change result back to unhandled
                        context.Result = default;
                        continue;
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// Handles exception by fallback request delegate.
        /// </summary>
        private async ValueTask HandleByFallbackRequestDelegate(
            ExceptionHandlingContext context,
            HttpContext httpContext)
        {
            if (Options.FallbackErrorResponseFactory == null)
            {
                return;
            }

            await Options.FallbackErrorResponseFactory(httpContext);
            context.Result = ExceptionHandlingResult.Handled();
        }
    }
}
