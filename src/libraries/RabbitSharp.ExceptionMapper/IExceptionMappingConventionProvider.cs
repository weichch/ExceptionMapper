using System.Collections.Generic;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Defines interface which provisions mapping conventions for scheme.
    /// </summary>
    public interface IExceptionMappingConventionProvider
    {
        /// <summary>
        /// Creates instances of mapping conventions registered to the specified scheme.
        /// </summary>
        /// <param name="scheme">The name of the scheme to which the mapping conventions belong.</param>
        IEnumerable<object> GetConventions<TOptions>(string scheme)
            where TOptions : ExceptionMappingSchemeOptions;

        /// <summary>
        /// Returns the mapping convention registration for the convention instance.
        /// </summary>
        /// <param name="convention">The convention instance which was provisioned by <see cref="GetConventions{TOptions}"/> method.</param>
        ExceptionMappingConventionRegistration? GetConventionRegistration(object convention);
    }
}
