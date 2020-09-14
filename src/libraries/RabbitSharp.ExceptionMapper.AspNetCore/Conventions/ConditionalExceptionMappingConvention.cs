using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RabbitSharp.Diagnostics.Builder;

namespace RabbitSharp.Diagnostics.AspNetCore.Conventions
{
    /// <summary>
    /// Provides conditional exception mapping based on <see cref="ConventionPredicateContext"/>. 
    /// </summary>
    public abstract class ConditionalExceptionMappingConvention : IEndpointExceptionMappingConvention
    {
        /// <summary>
        /// Initializes the mapping convention.
        /// </summary>
        /// <param name="predicateContext">The predicate context.</param>
        protected ConditionalExceptionMappingConvention(ConventionPredicateContext predicateContext)
        {
            PredicateContext = predicateContext ?? throw new ArgumentNullException(nameof(predicateContext));
        }

        /// <summary>
        /// Gets the predicate context.
        /// </summary>
        protected ConventionPredicateContext PredicateContext { get; }

        /// <summary>
        /// Runs predicate function and executes convention when exception mapping context meets criteria.
        /// </summary>
        /// <param name="context">The exception handling context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        public Task ExecuteAsync(ExceptionHandlingContext context, HttpContext httpContext)
        {
            if (PredicateContext.HasConditions
                &&!PredicateContext.CanHandleException(context, httpContext))
            {
                return OnSkipExecutionAsync(context, httpContext);
            }

            return OnExecutionAsync(context, httpContext);
        }

        /// <summary>
        /// In derived classes, execute the convention.
        /// </summary>
        /// <param name="context">The exception handling context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        protected abstract Task OnExecutionAsync(ExceptionHandlingContext context, HttpContext httpContext);

        /// <summary>
        /// Invoked when exception mapping context does not meet criteria.
        /// </summary>
        /// <param name="context">The exception handling context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        protected virtual Task OnSkipExecutionAsync(ExceptionHandlingContext context, HttpContext httpContext)
        {
            return Task.CompletedTask;
        }
    }
}
