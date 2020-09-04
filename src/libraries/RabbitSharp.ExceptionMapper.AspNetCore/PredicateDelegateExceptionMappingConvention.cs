using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Represents an exception mapping convention which delegates exception handling to
    /// another exception mapping convention.
    /// </summary>
    public class PredicateDelegateExceptionMappingConvention : DelegateExceptionMappingConvention
    {
        private readonly Func<ExceptionHandlingContext, HttpContext, bool> _predicate;

        /// <summary>
        /// Creates an instance of the mapping convention.
        /// </summary>
        /// <param name="predicate">The predicate function.</param>
        /// <param name="mappingDelegate">Another exception mapping convention.</param>
        public PredicateDelegateExceptionMappingConvention(
            Func<ExceptionHandlingContext, HttpContext, bool> predicate,
            EndpointExceptionMappingDelegate mappingDelegate)
            : base(mappingDelegate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// Runs exception mapping when exception mapping context meets criteria.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        public override Task ExecuteAsync(ExceptionHandlingContext context, HttpContext httpContext)
        {
            if (!_predicate(context, httpContext))
            {
                return Task.CompletedTask;
            }

            return base.ExecuteAsync(context, httpContext);
        }
    }
}
