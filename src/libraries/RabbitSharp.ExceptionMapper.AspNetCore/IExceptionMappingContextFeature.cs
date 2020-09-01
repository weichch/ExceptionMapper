using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Provides access to exception mapping context in <see cref="RequestDelegate"/>.
    /// </summary>
    public interface IExceptionMappingContextFeature
    {
        /// <summary>
        /// Gets or sets the exception handling context.
        /// </summary>
        ExceptionHandlingContext? Context { get; set; }
    }
}
