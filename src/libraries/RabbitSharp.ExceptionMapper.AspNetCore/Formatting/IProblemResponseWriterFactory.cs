using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics.AspNetCore.Formatting
{
    /// <summary>
    /// Provides ability to create <see cref="IProblemResponseWriter"/> for a request.
    /// </summary>
    public interface IProblemResponseWriterFactory
    {
        /// <summary>
        /// Creates <see cref="IProblemResponseWriter"/> for specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        IProblemResponseWriter Create(HttpContext httpContext);
    }
}
