using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;

namespace RabbitSharp.Diagnostics.AspNetCore.Internals
{
    /// <summary>
    /// Defines type to format route pattern with route values.
    /// </summary>
    public interface IRoutePatternFormatter
    {
        /// <summary>
        /// Formats route pattern with ambient and specified route values.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="pattern">The route pattern.</param>
        /// <param name="routeValues">The route values.</param>
        public string? Format(HttpContext httpContext, RoutePattern pattern, object? routeValues);
    }
}
