using System;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods to <see cref="IExceptionMappingConventionBuilder"/>
    /// and implementer types.
    /// </summary>
    public static class ExceptionMappingConventionBuilderExtensions
    {
        /// <summary>
        /// Sets order of the convention.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="order">The order of the convention.</param>
        public static TBuilder UseOrder<TBuilder>(this TBuilder builder, int order)
            where TBuilder : IExceptionMappingConventionBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Order = order;
            return builder;
        }

        /// <summary>
        /// Sets user-defined tags.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="tags">The user defined tags.</param>
        public static TBuilder UseTags<TBuilder>(this TBuilder builder, params string[] tags)
            where TBuilder : IExceptionMappingConventionBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Tags = tags;
            return builder;
        }
    }
}
