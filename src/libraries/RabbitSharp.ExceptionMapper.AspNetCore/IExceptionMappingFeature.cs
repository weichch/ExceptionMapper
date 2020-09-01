using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Provides access to data used by the exception mapping middleware and endpoint
    /// exception mapping scheme.
    /// </summary>
    public interface IExceptionMappingFeature
    {
        /// <summary>
        /// Gets the original endpoint.
        /// </summary>
        Endpoint? Endpoint { get; }

        /// <summary>
        /// Gets original route values.
        /// </summary>
        RouteValueDictionary? RouteValues { get; }

        /// <summary>
        /// Gets the request pipeline for re-running the current request.
        /// </summary>
        RequestDelegate RequestPipeline { get; }

        /// <summary>
        /// Gets the original request path.
        /// </summary>
        PathString RequestPath { get; }

        /// <summary>
        /// Gets the original request path base.
        /// </summary>
        PathString RequestPathBase { get; }
    }
}
