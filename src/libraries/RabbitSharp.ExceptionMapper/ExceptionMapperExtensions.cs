using System;
using System.Threading.Tasks;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Provides extension methods to <see cref="IExceptionMapper"/>.
    /// </summary>
    public static class ExceptionMapperExtensions
    {
        /// <summary>
        /// Executes registered exception mapping schemes and returns an <see cref="ExceptionHandlingResult"/>.
        /// </summary>
        /// <param name="mapper">The exception mapper.</param>
        /// <param name="exception">The exception to handle.</param>
        public static Task<ExceptionHandlingResult> MapAsync(
            this IExceptionMapper mapper,
            Exception exception)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            return mapper.MapAsync(exception, null);
        }

        /// <summary>
        /// Handles exception using registered schemes and executes the handling result.
        /// </summary>
        /// <param name="mapper">The exception mapper.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="context">The data context shared by all components used by the mapper.</param>
        public static async Task<object?> MapExceptionAsync(
            this IExceptionMapper mapper,
            Exception exception,
            ExceptionMappingContext? context = null)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var result = await mapper.MapAsync(exception, context);
            return result.GetResult();
        }
    }
}
