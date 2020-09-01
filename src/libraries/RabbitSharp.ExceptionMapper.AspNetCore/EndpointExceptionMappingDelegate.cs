using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Defines an exception mapping function for endpoint exception mapping scheme.
    /// </summary>
    /// <param name="context">The exception handling context.</param>
    /// <param name="httpContext">The HTTP context.</param>
    public delegate Task EndpointExceptionMappingDelegate(ExceptionHandlingContext context, HttpContext httpContext);
}
