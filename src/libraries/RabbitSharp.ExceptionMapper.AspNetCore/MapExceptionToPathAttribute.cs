using System;
using Microsoft.AspNetCore.Routing.Patterns;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Defines an exception mapping convention which maps exception of specified type to another endpoint at specified path.
    /// </summary>
    public class MapExceptionToPathAttribute : Attribute, IExceptionMappingMetadata
    {
        /// <summary>
        /// Creates an instance of the attribute.
        /// </summary>
        /// <param name="exceptionType">The type of the exception.</param>
        /// <param name="pattern">The route pattern.</param>
        public MapExceptionToPathAttribute(Type exceptionType, string pattern)
        {
            ExceptionType = exceptionType ?? throw new ArgumentNullException(nameof(exceptionType));

            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            try
            {
                RoutePattern = RoutePatternFactory.Parse(pattern);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid route pattern.", nameof(pattern), ex);
            }
        }

        /// <summary>
        /// Gets the type of target exception.
        /// </summary>
        public Type ExceptionType { get; }

        /// <summary>
        /// Gets the route pattern.
        /// </summary>
        public RoutePattern RoutePattern { get; }
    }
}
