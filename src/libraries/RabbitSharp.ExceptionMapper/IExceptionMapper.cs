using System;
using System.Threading.Tasks;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Defines an exception handling type which executes registered schemes.
    /// </summary>
    public interface IExceptionMapper
    {
        /// <summary>
        /// Executes registered exception mapping schemes and returns an <see cref="ExceptionHandlingResult"/>.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="context">The data shared by all components in the mapper.</param>
        Task<ExceptionHandlingResult> MapAsync(Exception exception, ExceptionMappingContext? context);
    }
}
