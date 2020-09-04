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
        /// <param name="options">The options.</param>
        /// <param name="configure">The action to configure mapping convention.</param>
        public static TOptions MapExceptions<TOptions>(
            this TOptions options,
            Action<ExceptionMappingConventionsBuilder> configure)
            where TOptions : ExceptionMappingSchemeOptions
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var mappingBuilder = new ExceptionMappingConventionsBuilder(options.Conventions);
            configure(mappingBuilder);
            mappingBuilder.Build();

            return options;
        }
    }
}
