using System;
using System.Collections.Generic;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Contains data for exception handling shared by handler and associated mapping conventions.
    /// </summary>
    public class ExceptionHandlingContext : IExceptionMappingDataContext
    {
        /// <summary>
        /// Creates an instance of the context.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="mappingContext">The data shared by all components within current mapping.</param>
        public ExceptionHandlingContext(
            Exception exception,
            ExceptionMappingContext mappingContext)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            ParentContext = mappingContext ?? throw new ArgumentNullException(nameof(mappingContext));
            Data = new Dictionary<object, object>();
        }

        /// <summary>
        /// Gets the captured exception to be handled.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the data shared by all components within current mapping.
        /// </summary>
        public ExceptionMappingContext ParentContext { get; }

        /// <summary>
        /// Gets the data on the context.
        /// </summary>
        public IDictionary<object, object> Data { get; }

        /// <summary>
        /// Gets or sets a predicate to filter mapping conventions to run.
        /// </summary>
        public Func<ExceptionMappingConventionRegistration, bool>? ConventionFilter { get; set; }

        /// <summary>
        /// Gets or sets the exception handling result.
        /// </summary>
        public ExceptionHandlingResult Result { get; set; }
    }
}
