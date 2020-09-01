using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RabbitSharp.Diagnostics.AspNetCore.Filters
{
    /// <summary>
    /// Configures exception mapping for an endpoint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MapExceptionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Creates an instance of the attribute.
        /// </summary>
        /// <param name="tags">The tags used to filter mapping conventions for the endpoint.</param>
        public MapExceptionAttribute(params string[] tags)
        {
            Tags = new HashSet<string>(tags ?? Enumerable.Empty<string>());
        }

        /// <summary>
        /// Gets a collection of tags for filtering exception mapping conventions.
        /// </summary>
        public ISet<string> Tags { get; }

        /// <summary>
        /// Indicates whether to exclude the endpoint from exception mapping.
        /// </summary>
        public bool Exclude { get; set; }
    }
}
