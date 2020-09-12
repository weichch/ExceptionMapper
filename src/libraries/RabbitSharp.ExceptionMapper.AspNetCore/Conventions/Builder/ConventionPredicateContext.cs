using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Represents context for convention to determine whether exception handing context
    /// meets criteria.
    /// </summary>
    public class ConventionPredicateContext
    {
        /// <summary>
        /// Creates an instance of the context.
        /// </summary>
        /// <param name="exceptionType">The exception type.</param>
        /// <param name="predicate">The predicate function.</param>
        public ConventionPredicateContext(
            Type? exceptionType,
            Func<ExceptionHandlingContext, HttpContext, bool>? predicate)
        {
            ExceptionType = exceptionType;
            Predicate = predicate;
        }

        /// <summary>
        /// Gets the exception type.
        /// </summary>
        public Type? ExceptionType { get; }

        /// <summary>
        /// Gets the actual meaningful exception type.
        /// </summary>
        public Type ActualExceptionType => ExceptionType ?? typeof(Exception);

        /// <summary>
        /// Gets the predicate function.
        /// </summary>
        public Func<ExceptionHandlingContext, HttpContext, bool>? Predicate { get; }

        /// <summary>
        /// Indicates whether this context has meaningful conditions.
        /// </summary>
        public bool HasConditions => ActualExceptionType != typeof(Exception) || Predicate != null;

        /// <summary>
        /// Validates whether the exception mapping context meets criteria.
        /// </summary>
        /// <param name="context">The exception mapping context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        public bool CanHandleException(ExceptionHandlingContext context, HttpContext httpContext)
        {
            if (ActualExceptionType != typeof(Exception))
            {
                if (!ActualExceptionType.IsInstanceOfType(context.Exception))
                {
                    return false;
                }
            }

            if (Predicate != null)
            {
                if (!Predicate(context, httpContext))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a conditional <see cref="EndpointExceptionMappingDelegate"/> which runs
        /// when exception mapping context meets criteria.
        /// </summary>
        /// <param name="innerDelegate">The inner delegate function.</param>
        public EndpointExceptionMappingDelegate Wrap(EndpointExceptionMappingDelegate innerDelegate)
        {
            if (innerDelegate == null)
            {
                throw new ArgumentNullException(nameof(innerDelegate));
            }

            if (!HasConditions)
            {
                return innerDelegate;
            }

            return (context, httpContext) =>
            {
                if (!CanHandleException(context, httpContext))
                {
                    return Task.CompletedTask;
                }

                return innerDelegate(context, httpContext);
            };
        }
    }
}
