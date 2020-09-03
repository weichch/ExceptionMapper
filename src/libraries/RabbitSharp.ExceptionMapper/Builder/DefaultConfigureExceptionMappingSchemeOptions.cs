using Microsoft.Extensions.Options;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Post configuration for scheme options.
    /// </summary>
    /// <typeparam name="TOptions">The type of the scheme options.</typeparam>
    class DefaultConfigureExceptionMappingSchemeOptions<TOptions> : IPostConfigureOptions<TOptions>
        where TOptions : ExceptionMappingSchemeOptions
    {
        public void PostConfigure(string name, TOptions options)
        {
            options.SchemeName = name;
        }
    }
}
