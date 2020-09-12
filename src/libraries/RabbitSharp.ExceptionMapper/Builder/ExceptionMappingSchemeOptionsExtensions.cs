using System;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods to build exception mapping conventions.
    /// </summary>
    public static class ExceptionMappingSchemeOptionsExtensions
    {
        /// <summary>
        /// Configures exception mapping conventions using a builder.
        /// </summary>
        /// <typeparam name="TOptions">The type of options.</typeparam>
        /// <typeparam name="TBuilder">The type of the builder.</typeparam>
        /// <param name="options">The options.</param>
        /// <param name="builder">The conventions builder.</param>
        /// <param name="configure">The action to configure mapping convention.</param>
        public static TOptions MapExceptions<TOptions, TBuilder>(
            this TOptions options,
            TBuilder builder,
            Action<TBuilder> configure)
            where TOptions : ExceptionMappingSchemeOptions
            where TBuilder : IExceptionMappingConventionsBuilder
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure(builder);
            builder.Build();

            return options;
        }
    }
}
