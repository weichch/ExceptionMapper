using System.Collections.Generic;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Defines a data context for exception mapping components.
    /// </summary>
    public interface IExceptionMappingDataContext
    {
        /// <summary>
        /// Gets the data on the context.
        /// </summary>
        IDictionary<object,object> Data { get; }
    }
}
