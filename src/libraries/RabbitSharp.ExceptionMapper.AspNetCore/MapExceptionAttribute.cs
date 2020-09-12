using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Enables exception mapping by selected conventions for the class or method. When selecting conventions,
    /// if tags defined on a convention overlaps with the tags defined in this attribute, the convention is
    /// selected. If multiple instances of the attribute applied, the convention must meet criteria for each
    /// of the attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MapExceptionAttribute : Attribute, IExceptionMappingConventionFilter
    {
        /// <summary>
        /// Creates an instance of the attribute.
        /// </summary>
        /// <param name="tags">The tags used to filter mapping conventions for the endpoint.</param>
        public MapExceptionAttribute(params string[] tags)
        {
            Tags = new HashSet<string>(tags ?? Enumerable.Empty<string>(), StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets tags for filtering conventions. If this collection overlaps with tags on a convention,
        /// the convention is selected.
        /// </summary>
        public ISet<string> Tags { get; }
    }
}
