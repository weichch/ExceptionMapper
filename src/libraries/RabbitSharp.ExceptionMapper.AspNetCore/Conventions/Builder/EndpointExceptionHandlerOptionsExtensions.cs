using System;
using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods to <see cref="EndpointExceptionHandlerOptionsExtensions"/>.
    /// </summary>
    public static class EndpointExceptionHandlerOptionsExtensions
    {
        /// <summary>
        /// Builds endpoint exception mapping conventions using a builder.
        /// </summary>
        /// <typeparam name="TOptions">The options type.</typeparam>
        /// <param name="options">The options.</param>
        /// <param name="configure">The action to configure mapping conventions.</param>
        public static TOptions MapEndpointExceptions<TOptions>(
            this TOptions options,
            Action<EndpointExceptionMappingConventionsBuilder> configure)
            where TOptions : EndpointExceptionHandlerOptions
            => options.MapExceptions(
                new EndpointExceptionMappingConventionsBuilder(options),
                configure);
    }
}
