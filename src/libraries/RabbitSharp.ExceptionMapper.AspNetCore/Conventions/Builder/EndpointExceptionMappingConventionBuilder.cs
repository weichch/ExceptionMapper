using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using RabbitSharp.Diagnostics.AspNetCore;
using RabbitSharp.Diagnostics.AspNetCore.Conventions;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Represents builder of endpoint exception mapping convention.
    /// </summary>
    public class EndpointExceptionMappingConventionBuilder : IExceptionMappingConventionBuilder
    {
        /// <summary>
        /// Creates an instance of the builder.
        /// </summary>
        /// <param name="schemeName">The name of the exception mapping scheme.</param>
        public EndpointExceptionMappingConventionBuilder(string schemeName)
        {
            SchemeName = schemeName ?? throw new ArgumentNullException(nameof(schemeName));
        }

        /// <summary>
        /// Gets the name of the exception mapping scheme.
        /// </summary>
        public string SchemeName { get; }

        /// <summary>
        /// Gets or sets the order of the mapping convention.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets user-defined tags on the mapping convention.
        /// </summary>
        public IEnumerable<string>? Tags { get; set; }

        /// <summary>
        /// Gets or sets the exception type to map.
        /// </summary>
        public Type? ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets predicate function.
        /// </summary>
        public Func<ExceptionHandlingContext, HttpContext, bool>? Predicate { get; set; }

        /// <summary>
        /// Gets or sets the mapping delegate.
        /// </summary>
        public EndpointExceptionMappingDelegate? MappingDelegate { get; set; }

        /// <summary>
        /// Gets or sets the action to configure conventions.
        /// </summary>
        public Action<ExceptionMappingConventionCollection>? BuildAction { get; set; }

        /// <summary>
        /// Builds the exception mapping convention.
        /// </summary>
        public void Build(ExceptionMappingConventionCollection conventions)
        {
            if (BuildAction != null)
            {
                BuildAction.Invoke(conventions);
            }
            else if (MappingDelegate != null)
            {
                var predicateContext = CreateConventionPredicateContext();
                var convention = new DelegateExceptionMappingConvention(predicateContext.Wrap(MappingDelegate));
                conventions.AddConvention(convention, Tags, Order);
            }
            else
            {
                // Use default convention
                var predicateContext = CreateConventionPredicateContext();
                conventions.AddParameterized<DefaultExceptionMappingConventionConvention>(
                    Tags, Order, SchemeName, predicateContext);
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="ConventionPredicateContext"/>.
        /// </summary>
        public ConventionPredicateContext CreateConventionPredicateContext()
        {
            return new ConventionPredicateContext(ExceptionType, Predicate);
        }
    }
}
