using System;
using System.Collections.Generic;
using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Represents builder of endpoint exception mapping convention.
    /// </summary>
    public class EndpointExceptionMappingConventionBuilder : IEndpointExceptionMappingConventionBuilder
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
        /// Gets or sets the mapping delegate.
        /// </summary>
        public EndpointExceptionMappingDelegate? MappingDelegate { get; set; }

        /// <summary>
        /// Gets or sets the convention instance.
        /// </summary>
        public IEndpointExceptionMappingConvention? Convention { get; set; }

        /// <summary>
        /// Gets or sets the action to configure conventions.
        /// </summary>
        public Action<ExceptionMappingConventionCollection, EndpointExceptionMappingConventionBuilder>?
            BuildAction { get; set; }

        /// <summary>
        /// Builds the exception mapping convention.
        /// </summary>
        /// <param name="conventions">The target conventions.</param>
        public void Build(ExceptionMappingConventionCollection conventions)
        {
            if (MappingDelegate != null)
            {
                conventions.AddConvention(new DelegateExceptionMappingConvention(MappingDelegate), Tags, Order);
            }
            else if (Convention != null)
            {
                conventions.AddConvention(Convention, Tags, Order);
            }
            else if (BuildAction != null)
            {
                BuildAction(conventions, this);
            }
            else
            {
                throw new InvalidOperationException("Cannot build an endpoint exception mapping convention.");
            }
        }
    }
}
