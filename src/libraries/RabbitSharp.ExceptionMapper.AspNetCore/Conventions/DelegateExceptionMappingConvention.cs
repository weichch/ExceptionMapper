using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics.AspNetCore.Conventions
{
    /// <summary>
    /// Represents an exception mapping convention which delegates exception handling to
    /// another exception mapping convention.
    /// </summary>
    public class DelegateExceptionMappingConvention : IEndpointExceptionMappingConvention
    {
        private readonly EndpointExceptionMappingDelegate _mappingDelegate;

        /// <summary>
        /// Creates an instance of the mapping convention.
        /// </summary>
        /// <param name="mappingDelegate">Another exception mapping convention.</param>
        public DelegateExceptionMappingConvention(EndpointExceptionMappingDelegate mappingDelegate)
        {
            _mappingDelegate = mappingDelegate ?? throw new ArgumentNullException(nameof(mappingDelegate));
        }

        /// <summary>
        /// Executes exception handling in another exception mapping convention.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        public virtual Task ExecuteAsync(ExceptionHandlingContext context, HttpContext httpContext)
            => _mappingDelegate(context, httpContext);

        /// <summary>
        /// Creates a conditional <see cref="DelegateExceptionMappingConvention"/> which runs
        /// when exception mapping context meets criteria.
        /// </summary>
        /// <param name="innerDelegate">The inner delegate function.</param>
        /// <param name="exceptionType">The exception type.</param>
        /// <param name="predicate">The predicate function.</param>
        public static DelegateExceptionMappingConvention CreateConditional(
            EndpointExceptionMappingDelegate innerDelegate,
            Type? exceptionType,
            Func<ExceptionHandlingContext, HttpContext, bool>? predicate)
        {
            if (innerDelegate == null)
            {
                throw new ArgumentNullException(nameof(innerDelegate));
            }

            var actualExceptionType = exceptionType ?? typeof(Exception);

            // No conditions
            if (actualExceptionType == typeof(Exception) && predicate == null)
            {
                return new DelegateExceptionMappingConvention(innerDelegate);
            }

            var filteredDelegate = innerDelegate;
            if (actualExceptionType != typeof(Exception))
            {
                filteredDelegate = FilterByExceptionType(filteredDelegate, actualExceptionType);
            }

            if (predicate != null)
            {
                filteredDelegate = FilterByPredicate(filteredDelegate, predicate);
            }

            return new DelegateExceptionMappingConvention(filteredDelegate);

            static EndpointExceptionMappingDelegate FilterByExceptionType(
                EndpointExceptionMappingDelegate inner,
                Type exceptionType)
            {
                return (context, httpContext) =>
                {
                    if (!exceptionType.IsInstanceOfType(context.Exception))
                    {
                        return Task.CompletedTask;
                    }

                    return inner(context, httpContext);
                };
            }

            static EndpointExceptionMappingDelegate FilterByPredicate(
                EndpointExceptionMappingDelegate inner,
                Func<ExceptionHandlingContext, HttpContext, bool> predicate)
            {
                return (context, httpContext) =>
                {
                    if (!predicate(context, httpContext))
                    {
                        return Task.CompletedTask;
                    }

                    return inner(context, httpContext);
                };
            }
        }
    }
}
