using System;
using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Provides extension methods to data context.
    /// </summary>
    public static class EndpointExceptionMappingContextExtensions
    {
        /// <summary>
        /// Gets <see cref="HttpContext"/> from the data context.
        /// </summary>
        /// <param name="context">The data context.</param>
        public static HttpContext? GetHttpContext(this IExceptionMappingDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Data.TryGetValue(typeof(HttpContext), out var httpContext);
            return httpContext as HttpContext;
        }

        /// <summary>
        /// Gets <see cref="HttpContext"/> from the data context.
        /// </summary>
        /// <param name="context">The data context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        public static void SetHttpContext(this IExceptionMappingDataContext context, HttpContext httpContext)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            context.Data[typeof(HttpContext)] = httpContext;
        }
    }
}
