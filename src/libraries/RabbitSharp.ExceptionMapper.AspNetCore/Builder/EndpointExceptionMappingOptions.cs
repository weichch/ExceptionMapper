using RabbitSharp.Diagnostics;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Represents settings for exception mapper in exception handling/mapping middleware.
    /// </summary>
    public class EndpointExceptionMappingOptions
    {
        /// <summary>
        /// Gets or sets the exception mapper to be used in the middleware. If this property is set to <c>null</c>,
        /// an instance of <see cref="IExceptionMapper"/> is created for each exception mapping. <see cref="IExceptionMapper"/>
        /// is usually expensive to create, therefore it is recommended to always provide a cached <see cref="IExceptionMapper"/>
        /// instance.
        /// </summary>
        public IExceptionMapper? Mapper { get; set; }

        /// <summary>
        /// Gets or sets the application builder.
        /// </summary>
        public IApplicationBuilder ApplicationBuilder { get; set; } = null!;
    }
}
