using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Represents an exception mapping convention which delegates exception handling to
    /// another exception mapping convention.
    /// </summary>
    public class DelegateExceptionMappingConvention : IEndpointExceptionMappingConvention
    {
        private readonly EndpointExceptionMappingDelegate _mappingDelegate;

        /// <summary>
        /// Creates an instance of the mapping convention.
        /// </summary>
        /// <param name="mappingDelegate">Another exception mapping convention.</param>
        public DelegateExceptionMappingConvention(EndpointExceptionMappingDelegate mappingDelegate)
        {
            _mappingDelegate = mappingDelegate ?? throw new ArgumentNullException(nameof(mappingDelegate));
        }

        /// <summary>
        /// Executes exception handling in another exception mapping convention.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        public virtual Task ExecuteAsync(ExceptionHandlingContext context, HttpContext httpContext)
            => _mappingDelegate(context, httpContext);
    }
}
