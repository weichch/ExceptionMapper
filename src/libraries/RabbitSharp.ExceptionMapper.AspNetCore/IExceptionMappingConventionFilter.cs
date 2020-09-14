using System.Collections.Generic;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Defines exception mapping convention filter metadata.
    /// </summary>
    public interface IExceptionMappingConventionFilter : IExceptionMappingMetadata
    {
        /// <summary>
        /// Gets a collection of tags for filtering exception mapping conventions.
        /// </summary>
        ISet<string> Tags { get; }
    }
}
