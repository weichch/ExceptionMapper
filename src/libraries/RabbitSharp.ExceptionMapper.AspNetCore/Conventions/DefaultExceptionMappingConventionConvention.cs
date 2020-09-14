using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RabbitSharp.Diagnostics.Builder;

namespace RabbitSharp.Diagnostics.AspNetCore.Conventions
{
    /// <summary>
    /// Executes the default exception mapping convention.
    /// </summary>
    class DefaultExceptionMappingConventionConvention : ConditionalExceptionMappingConvention
    {
        private readonly EndpointExceptionHandlerOptions _options;

        public DefaultExceptionMappingConventionConvention(
            IOptionsMonitor<EndpointExceptionHandlerOptions> options,
            string schemeName,
            ConventionPredicateContext predicateContext)
            : base(predicateContext)
        {
            _options = options.Get(schemeName);
        }

        protected override Task OnExecutionAsync(ExceptionHandlingContext context, HttpContext httpContext)
        {
            var convention = _options.DefaultConvention;
            if (convention == null)
            {
                return Task.CompletedTask;
            }

            return convention.ExecuteAsync(context, httpContext);
        }
    }
}
