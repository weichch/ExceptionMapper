using Microsoft.AspNetCore.Http;
using RabbitSharp.Diagnostics.AspNetCore.Filters;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Represents settings for <see cref="EndpointExceptionHandler"/>.
    /// </summary>
    public class EndpointExceptionHandlerOptions : ExceptionMappingSchemeOptions
    {
        /// <summary>
        /// Indicates whether to enable exception mapping for all endpoints in the application. If this
        /// property is set to <c>false</c>, <see cref="MapExceptionAttribute"/> must be used to enable
        /// exception mapping for endpoints. Default value is <c>true</c>.
        /// </summary>
        public bool ImplicitExceptionMapping { get; set; } = true;

        /// <summary>
        /// Gets or sets a fallback response factory which produces an error response when all registered
        /// mapping conventions have failed to handle the exception.
        /// </summary>
        public RequestDelegate? FallbackErrorResponseFactory { get; set; }
    }
}
