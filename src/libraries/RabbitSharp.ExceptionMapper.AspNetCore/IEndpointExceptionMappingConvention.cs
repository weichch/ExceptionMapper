using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Defines exception mapping convention for endpoint exception mapping scheme.
    /// </summary>
    public interface IEndpointExceptionMappingConvention
    {
        /// <summary>
        /// Handles exception.
        /// </summary>
        /// <param name="context">The exception handling context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        Task ExecuteAsync(ExceptionHandlingContext context, HttpContext httpContext);
    }
}
