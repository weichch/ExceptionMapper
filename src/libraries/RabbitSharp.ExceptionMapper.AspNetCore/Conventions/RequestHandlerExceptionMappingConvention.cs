using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RabbitSharp.Diagnostics.Builder;

namespace RabbitSharp.Diagnostics.AspNetCore.Conventions
{
    /// <summary>
    /// Implements <see cref="IEndpointExceptionMappingConvention"/> which maps exception
    /// to a <see cref="RequestDelegate"/>.
    /// </summary>
    class RequestHandlerExceptionMappingConvention : ConditionalExceptionMappingConvention
    {
        private readonly RequestDelegate _requestHandler;

        public RequestHandlerExceptionMappingConvention(
            RequestDelegate requestHandler,
            ConventionPredicateContext predicateContext)
            : base(predicateContext)
        {
            _requestHandler = requestHandler;
        }

        protected override async Task OnExecutionAsync(ExceptionHandlingContext context, HttpContext httpContext)
        {
            await _requestHandler(httpContext);
            if (!context.Result.IsHandled)
            {
                context.Result = ExceptionHandlingResult.Handled();
            }
        }
    }
}
