using System;
using System.Threading.Tasks;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Defines a type which handles exception for an exception mapping scheme.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Handles an exception.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="mappingContext">The data shared by all components in the mapper.</param>
        ValueTask<ExceptionHandlingResult> HandleExceptionAsync(
            Exception exception,
            ExceptionMappingContext mappingContext);

        /// <summary>
        /// Initializes the exception handler for the specified scheme.
        /// </summary>
        /// <param name="name">The name of the scheme.</param>
        ValueTask InitializeAsync(string name);
    }
}
