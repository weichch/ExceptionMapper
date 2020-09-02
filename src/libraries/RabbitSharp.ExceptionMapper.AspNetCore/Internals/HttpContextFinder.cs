using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitSharp.Diagnostics.AspNetCore.Internals
{
    /// <summary>
    /// Implements <see cref="IHttpContextFinder"/>.
    /// </summary>
    class HttpContextFinder : IHttpContextFinder
    {
        private readonly IServiceProvider _serviceProvider;

        public HttpContextFinder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public HttpContext? GetHttpContext(ExceptionMappingContext context)
        {
            var httpContext = context.GetHttpContext();
            if (httpContext != null)
            {
                return httpContext;
            }

            // Find in IHttpContextAccessor if the application has it added to the service collection.
            var accessor = _serviceProvider.GetService<IHttpContextAccessor>();
            httpContext = accessor?.HttpContext;

            return httpContext;
        }
    }
}
