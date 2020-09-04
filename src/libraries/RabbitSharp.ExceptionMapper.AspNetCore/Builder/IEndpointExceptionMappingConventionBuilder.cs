using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Defines type which builds endpoint exception mapping convention.
    /// </summary>
    public interface IEndpointExceptionMappingConventionBuilder : IExceptionMappingConventionBuilder
    {
        /// <summary>
        /// Gets or sets the mapping delegate.
        /// </summary>
        EndpointExceptionMappingDelegate? MappingDelegate { get; set; }
    }
}
