using Microsoft.AspNetCore.Http;

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
        /// exception mapping for endpoints. To exclude class or method from exception mapping, use
        /// <see cref="ExcludeFromExceptionMappingAttribute"/>. Default value is <c>true</c>.
        /// </summary>
        public bool ImplicitExceptionMapping { get; set; } = true;

        /// <summary>
        /// Gets or sets the default exception mapping convention to execute when no convention provided
        /// while building an exception mapping convention.
        /// </summary>
        public IEndpointExceptionMappingConvention? DefaultConvention { get; set; }

        /// <summary>
        /// Gets or sets a fallback response factory which produces an error response when all registered
        /// mapping conventions have failed to handle the exception.
        /// </summary>
        public RequestDelegate? FallbackErrorResponseFactory { get; set; }
    }
}
