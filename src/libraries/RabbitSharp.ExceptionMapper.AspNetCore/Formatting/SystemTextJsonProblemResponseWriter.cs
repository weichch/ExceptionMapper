using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace RabbitSharp.Diagnostics.AspNetCore.Formatting
{
    /// <summary>
    /// Implements <see cref="IProblemResponseWriter"/> for endpoint using
    /// <see cref="System.Text.Json.JsonSerializer"/>.
    /// </summary>
    public class SystemTextJsonProblemResponseWriter : IProblemResponseWriter
    {
        /// <summary>
        /// Creates an instance of the writer.
        /// </summary>
        /// <param name="jsonOptions">The JSON options.</param>
        public SystemTextJsonProblemResponseWriter(IOptions<JsonOptions> jsonOptions)
        {
            SerializerOptions = jsonOptions.Value.JsonSerializerOptions;
        }

        /// <summary>
        /// Gets the serializer options.
        /// </summary>
        public JsonSerializerOptions SerializerOptions { get; }

        /// <summary>
        /// Writes <see cref="ProblemDetails"/> to response body.
        /// </summary>
        public async Task WriteAsync(ProblemResponseWriterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;
            if (response.HasStarted)
            {
                throw new InvalidOperationException("Response has started.");
            }

            var problem = context.Problem;

            response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            response.ContentType = "application/problem+json; charset=utf-8";

            context.ApplyTo(response);
            await response.StartAsync();

            var bodyStream = response.Body;
            await JsonSerializer.SerializeAsync(bodyStream, problem, problem.GetType(), SerializerOptions);
            await bodyStream.FlushAsync();
        }
    }
}
