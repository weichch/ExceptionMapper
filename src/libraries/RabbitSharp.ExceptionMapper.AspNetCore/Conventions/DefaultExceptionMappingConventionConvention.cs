using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RabbitSharp.Diagnostics.Builder;

namespace RabbitSharp.Diagnostics.AspNetCore.Conventions
{
    /// <summary>
    /// Executes the default exception mapping convention.
    /// </summary>
    class DefaultExceptionMappingConventionConvention : IEndpointExceptionMappingConvention
    {
        private readonly EndpointExceptionHandlerOptions _options;
        private readonly ConventionPredicateContext _predicateContext;

        public DefaultExceptionMappingConventionConvention(
            IOptionsMonitor<EndpointExceptionHandlerOptions> options,
            string schemeName,
            ConventionPredicateContext predicateContext)
        {
            _options = options.Get(schemeName);
            _predicateContext = predicateContext;
        }

        public Task ExecuteAsync(ExceptionHandlingContext context, HttpContext httpContext)
        {
            var convention = _options.DefaultConvention;

            if (convention != null
                && (!_predicateContext.HasConditions
                    || _predicateContext.CanHandleException(context, httpContext)))
            {
                return convention.ExecuteAsync(context, httpContext);
            }

            return Task.CompletedTask;
        }
    }
}
