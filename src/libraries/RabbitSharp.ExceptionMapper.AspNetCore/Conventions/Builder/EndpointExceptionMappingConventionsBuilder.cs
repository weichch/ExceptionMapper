using System;
using System.Collections.Generic;
using RabbitSharp.Diagnostics.AspNetCore;

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
        /// <param name="options">The exception handler options.</param>
        public EndpointExceptionMappingConventionsBuilder(EndpointExceptionHandlerOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            _conventionBuilders = new List<Action<ExceptionMappingConventionCollection>>();
        }

        /// <summary>
        /// Gets the exception handler options.
        /// </summary>
        public EndpointExceptionHandlerOptions Options { get; }

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
            var conventions = Options.Conventions;
            foreach (var conventionBuilder in _conventionBuilders)
            {
                conventionBuilder(conventions);
            }
        }
    }
}
