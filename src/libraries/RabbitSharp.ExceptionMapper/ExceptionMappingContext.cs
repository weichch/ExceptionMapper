using System.Collections.Generic;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Contains data shared by all components within a method call to <see cref="IExceptionMapper.MapAsync"/>.
    /// </summary>
    public class ExceptionMappingContext : IExceptionMappingDataContext
    {
        /// <summary>
        /// Creates an instance of the context.
        /// </summary>
        public ExceptionMappingContext()
        {
            Data = new Dictionary<object, object>();
        }

        /// <summary>
        /// Gets the data on the context.
        /// </summary>
        public IDictionary<object, object> Data { get; }
    }
}
