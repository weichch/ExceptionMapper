using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitSharp.Diagnostics.AspNetCore.Formatting
{
    /// <summary>
    /// Implements <see cref="IProblemResponseWriterFactory"/>.
    /// </summary>
    public class ProblemResponseWriterFactory : IProblemResponseWriterFactory
    {
        /// <summary>
        /// Creates <see cref="ObjectResultProblemResponseWriter"/> for controller endpoint,
        /// otherwise creates <see cref="SystemTextJsonProblemResponseWriter"/>.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        public IProblemResponseWriter Create(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var feature = httpContext.Features.Get<IExceptionMappingFeature>();
            var endpoint = feature?.Endpoint;
            var actionDescriptor = endpoint?.Metadata.GetMetadata<ActionDescriptor>();
            
            if (actionDescriptor != null)
            {
                return ActivatorUtilities.GetServiceOrCreateInstance<ObjectResultProblemResponseWriter>(
                    httpContext.RequestServices);
            }

            return ActivatorUtilities.GetServiceOrCreateInstance<SystemTextJsonProblemResponseWriter>(
                httpContext.RequestServices);
        }
    }
}
