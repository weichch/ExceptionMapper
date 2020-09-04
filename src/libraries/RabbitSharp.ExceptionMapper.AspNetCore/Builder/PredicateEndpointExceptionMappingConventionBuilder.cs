using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Represents builder of conditional exception mapping convention.
    /// </summary>
    public class PredicateEndpointExceptionMappingConventionBuilder : IEndpointExceptionMappingConventionBuilder
    {
        /// <summary>
        /// Gets or sets the order of the mapping convention.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets user-defined tags on the mapping convention.
        /// </summary>
        public IEnumerable<string>? Tags { get; set; }

        /// <summary>
        /// Gets or sets predicate function.
        /// </summary>
        public Func<ExceptionHandlingContext, HttpContext, bool> Predicate { get; set; } = null!;

        /// <summary>
        /// Gets or sets the mapping delegate.s
        /// </summary>
        public EndpointExceptionMappingDelegate? MappingDelegate { get; set; }

        /// <summary>
        /// Builds exception mapping convention.
        /// </summary>
        public void Build(ExceptionMappingConventionCollection conventions)
        {
            if (MappingDelegate == null || Predicate == null)
            {
                throw new InvalidOperationException("Cannot build an endpoint exception mapping convention.");
            }

            conventions.AddConvention(new PredicateDelegateExceptionMappingConvention(
                Predicate, MappingDelegate), Tags, Order);
        }
    }
}
