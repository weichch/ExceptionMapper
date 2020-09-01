using System;
using RabbitSharp.Diagnostics.AspNetCore.Filters;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Provides exception mapping extension methods to <see cref="IEndpointConventionBuilder"/>.
    /// </summary>
    public static class EndpointExceptionMappingEndpointConventionBuilderExtensions
    {
        /// <summary>
        /// Enables exception mapping using specified mapping conventions for the endpoint.
        /// </summary>
        /// <typeparam name="TBuilder">The convention builder type.</typeparam>
        /// <param name="builder">The convention builder.</param>
        /// <param name="tags">The tags used to filter conventions.</param>
        public static TBuilder MapException<TBuilder>(this TBuilder builder, params string[] tags)
            where TBuilder : IEndpointConventionBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Add(endpoint => endpoint.Metadata.Add(
                new MapExceptionAttribute(tags)));
            return builder;
        }

        /// <summary>
        /// Disables exception mapping for the endpoint.
        /// </summary>
        /// <typeparam name="TBuilder">The convention builder type.</typeparam>
        /// <param name="builder">The convention builder.</param>
        public static TBuilder ExcludeFromExceptionMapping<TBuilder>(this TBuilder builder)
            where TBuilder : IEndpointConventionBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Add(endpoint => endpoint.Metadata.Add(
                new MapExceptionAttribute {Exclude = true}));
            return builder;
        }
    }
}
