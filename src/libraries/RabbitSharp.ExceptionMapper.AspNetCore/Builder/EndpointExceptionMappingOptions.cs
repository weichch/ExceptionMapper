using System;
using System.Collections.Generic;
using RabbitSharp.Diagnostics;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Represents settings for exception mapper in exception handling/mapping middleware.
    /// </summary>
    public class EndpointExceptionMappingOptions
    {
        /// <summary>
        /// Creates an instance of the options.
        /// </summary>
        public EndpointExceptionMappingOptions()
        {
            Schemes = new HashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets or sets the exception mapper to be used in the middleware. If this property is set to <c>null</c>,
        /// an instance of <see cref="IExceptionMapper"/> is created for each exception mapping. <see cref="IExceptionMapper"/>
        /// is usually expensive to create, therefore it is recommended to always provide a cached <see cref="IExceptionMapper"/>
        /// instance.
        /// </summary>
        public IExceptionMapper? Mapper { get; set; }

        /// <summary>
        /// Gets a set of schemes to run from the middleware. By default, only default endpoint
        /// exception mapping scheme will run. If additional schemes need to run, add the scheme
        /// names to this collection.
        /// </summary>
        public ISet<string> Schemes { get; set; }
    }
}
