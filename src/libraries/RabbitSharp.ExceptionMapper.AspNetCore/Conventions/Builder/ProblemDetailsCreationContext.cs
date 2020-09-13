using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Represents context for creating <see cref="ProblemDetails"/>.
    /// </summary>
    public class ProblemDetailsCreationContext
    {
        private ProblemDetailsFactory? _factory;

        /// <summary>
        /// Creates an instance of the context.
        /// </summary>
        /// <param name="exceptionHandlingContext">The exception handling context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        public ProblemDetailsCreationContext(
            ExceptionHandlingContext exceptionHandlingContext,
            HttpContext httpContext)
        {
            ExceptionHandlingContext = exceptionHandlingContext;
            HttpContext = httpContext;
        }

        /// <summary>
        /// Gets the exception handling context.
        /// </summary>
        public ExceptionHandlingContext ExceptionHandlingContext { get; }

        /// <summary>
        /// Gets the HTTP context.
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// Gets problem details factory.
        /// </summary>
        public ProblemDetailsFactory Factory =>
            _factory ??= HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
    }
}
