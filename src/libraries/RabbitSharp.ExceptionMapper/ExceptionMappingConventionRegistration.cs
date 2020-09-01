using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Represents a registered exception mapping convention.
    /// </summary>
    public class ExceptionMappingConventionRegistration
    {
        /// <summary>
        /// Creates an instance of the mapping convention registration.
        /// </summary>
        /// <param name="conventionType">The type of the convention.</param>
        /// <param name="conventionFactory">The factory function for creating mapping convention instance.</param>
        /// <param name="tags">The user-defined tags.</param>
        public ExceptionMappingConventionRegistration(
            Type conventionType,
            Func<IServiceProvider, object> conventionFactory,
            IEnumerable<string>? tags)
        {
            if (conventionType == null)
            {
                throw new ArgumentNullException(nameof(conventionType));
            }

            if (conventionType.IsAbstract
                || conventionType.IsValueType
                || conventionType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Invalid mapping convention type.");
            }

            ConventionType = conventionType;
            ConventionFactory = conventionFactory ?? throw new ArgumentNullException(nameof(conventionFactory));
            Tags = new HashSet<string>(tags ?? Enumerable.Empty<string>(), StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets the type of the mapping convention.
        /// </summary>
        public Type ConventionType { get; }

        /// <summary>
        /// Gets the factory function for creating mapping convention instance.
        /// </summary>
        public Func<IServiceProvider, object> ConventionFactory { get; }

        /// <summary>
        /// Gets a set of user-defined tags.
        /// </summary>
        public ISet<string> Tags { get; }

        /// <summary>
        /// Gets or sets the order of the convention in the scheme.
        /// </summary>
        public int Order { get; set; }
    }
}
