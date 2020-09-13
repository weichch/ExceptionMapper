using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RabbitSharp.Diagnostics.AspNetCore.Formatting
{
    /// <summary>
    /// Defines type which writes <see cref="ProblemDetails"/> to HTTP response body.
    /// </summary>
    public interface IProblemResponseWriter
    {
        /// <summary>
        /// Writes <see cref="ProblemDetails"/> to HTTP response body.
        /// </summary>
        /// <param name="context">The writer context.</param>
        public Task WriteAsync(ProblemResponseWriterContext context);
    }
}
