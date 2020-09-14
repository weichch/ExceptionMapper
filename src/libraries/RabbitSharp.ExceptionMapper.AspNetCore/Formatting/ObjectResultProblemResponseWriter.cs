using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitSharp.Diagnostics.AspNetCore.Formatting
{
    /// <summary>
    /// Implements <see cref="IProblemResponseWriter"/> using
    /// <see cref="IActionResultExecutor{T}"/>.
    /// </summary>
    public class ObjectResultProblemResponseWriter : IProblemResponseWriter
    {
        /// <summary>
        /// Writes <see cref="ProblemDetails"/> to response body.
        /// </summary>
        public Task WriteAsync(ProblemResponseWriterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.ActionContext == null)
            {
                throw new InvalidOperationException("Action context is required.");
            }

            var executor = context.HttpContext.RequestServices
                .GetRequiredService<IActionResultExecutor<ObjectResult>>();
            var result = new ObjectResult(context.Problem)
            {
                StatusCode = context.Problem.Status
            };

            return executor.ExecuteAsync(context.ActionContext, result);
        }
    }
}
