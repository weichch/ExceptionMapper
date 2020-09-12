using System;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Defines a type for building exception mapping conventions for a scheme.
    /// </summary>
    public interface IExceptionMappingConventionsBuilder
    {
        /// <summary>
        /// Adds a convention builder.
        /// </summary>
        /// <param name="conventionBuilder">The action to build convention.</param>
        void Add(Action<ExceptionMappingConventionCollection> conventionBuilder);

        /// <summary>
        /// Builds exception mapping conventions.
        /// </summary>
        void Build();
    }
}
