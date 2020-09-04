using System.Collections.Generic;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Defines type which configures an exception mapping.
    /// </summary>
    public interface IExceptionMappingConventionBuilder
    {
        /// <summary>
        /// Gets or sets the order of the convention.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the user-defined tags.
        /// </summary>
        public IEnumerable<string>? Tags { get; set; }

        /// <summary>
        /// Builds the exception mapping convention.
        /// </summary>
        /// <param name="conventions">The target conventions.</param>
        void Build(ExceptionMappingConventionCollection conventions);
    }
}
