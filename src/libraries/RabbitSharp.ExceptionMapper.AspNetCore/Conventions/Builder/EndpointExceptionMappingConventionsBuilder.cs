using System;
using System.Collections.Generic;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Represents builder for endpoint exception mapping conventions.
    /// </summary>
    public class EndpointExceptionMappingConventionsBuilder : IExceptionMappingConventionsBuilder
    {
        private readonly List<Action<ExceptionMappingConventionCollection>> _conventionBuilders;

        /// <summary>
        /// Creates an instance of the builder.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        public EndpointExceptionMappingConventionsBuilder(ExceptionMappingConventionCollection conventions)
        {
            Conventions = conventions ?? throw new ArgumentNullException(nameof(conventions));
            _conventionBuilders = new List<Action<ExceptionMappingConventionCollection>>();
        }

        /// <summary>
        /// Gets the conventions.
        /// </summary>
        public ExceptionMappingConventionCollection Conventions { get; }

        /// <summary>
        /// Adds a convention builder.
        /// </summary>
        /// <param name="conventionBuilder">The action to build convention.</param>
        public void Add(Action<ExceptionMappingConventionCollection> conventionBuilder)
        {
            if (conventionBuilder == null)
            {
                throw new ArgumentNullException(nameof(conventionBuilder));
            }

            _conventionBuilders.Add(conventionBuilder);
        }

        /// <summary>
        /// Builds exception mapping conventions.
        /// </summary>
        public void Build()
        {
            foreach (var conventionBuilder in _conventionBuilders)
            {
                conventionBuilder(Conventions);
            }
        }
    }
}
