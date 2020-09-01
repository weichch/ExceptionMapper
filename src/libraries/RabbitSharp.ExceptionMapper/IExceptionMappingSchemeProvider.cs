using System.Collections.Generic;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Provides access to registered exception mapping schemes.
    /// </summary>
    public interface IExceptionMappingSchemeProvider
    {
        /// <summary>
        /// Gets the registered exception mapping schemes.
        /// </summary>
        IEnumerable<ExceptionMappingSchemeRegistration> GetSchemes();

        /// <summary>
        /// Gets registered exception mapping scheme by name.
        /// </summary>
        /// <param name="name">The name of the scheme.</param>
        ExceptionMappingSchemeRegistration? GetScheme(string name);
    }
}
