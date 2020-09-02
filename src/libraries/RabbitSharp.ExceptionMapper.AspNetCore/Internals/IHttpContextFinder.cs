using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics.AspNetCore.Internals
{
    /// <summary>
    /// Provides access to current <see cref="HttpContext"/> for the exception handling context.
    /// </summary>
    public interface IHttpContextFinder
    {
        /// <summary>
        /// Gets the most significant <see cref="HttpContext"/> instance.
        /// </summary>
        /// <param name="context">The exception mapping context.</param>
        HttpContext? GetHttpContext(ExceptionMappingContext context);
    }
}
