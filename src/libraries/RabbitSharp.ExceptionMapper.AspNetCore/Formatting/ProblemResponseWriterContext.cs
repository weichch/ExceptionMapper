using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RabbitSharp.Diagnostics.AspNetCore.Formatting
{
    /// <summary>
    /// Represents the context for <see cref="IProblemResponseWriter"/>.
    /// </summary>
    public class ProblemResponseWriterContext
    {
        private readonly List<Action<HttpResponse>> _callbacks = new List<Action<HttpResponse>>();

        /// <summary>
        /// Creates an instance of the context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="problem">The <see cref="ProblemDetails"/> object to be written.</param>
        public ProblemResponseWriterContext(HttpContext httpContext, ProblemDetails problem)
        {
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            Problem = problem ?? throw new ArgumentNullException(nameof(problem));
        }

        /// <summary>
        /// Gets the HTTP context.
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// Gets the action context if writing from MVC types.
        /// </summary>
        public ActionContext? ActionContext { get; set; }

        /// <summary>
        /// Gets the problem object.
        /// </summary>
        public ProblemDetails Problem { get; }

        /// <summary>
        /// Adds a callback function to be executed before writing to response.
        /// </summary>
        public void OnWriting(Action<HttpResponse> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            _callbacks.Add(callback);
        }

        /// <summary>
        /// Applies configuration to HTTP response.
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        public void ApplyTo(HttpResponse httpResponse)
        {
            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }

            foreach (var callback in _callbacks)
            {
                callback(httpResponse);
            }
        }
    }
}
