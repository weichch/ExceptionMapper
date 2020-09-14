using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Represents an exception handling result.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Handling) + "}")]
    public readonly struct ExceptionHandlingResult
    {
        private readonly ExceptionDispatchInfo? _sourceException;
        private readonly object? _result;

        /// <summary>
        /// Returns a void handled result.
        /// </summary>
        public static ExceptionHandlingResult Handled()
            => new ExceptionHandlingResult(ExceptionHandling.Handled,null,null);

        /// <summary>
        /// Returns a handled result which will rethrow the specified exception.
        /// </summary>
        /// <param name="exception">The exception to be rethrown.</param>
        public static ExceptionHandlingResult Rethrow(Exception exception)
            => new ExceptionHandlingResult(
                ExceptionHandling.Rethrow,
                ExceptionDispatchInfo.Capture(exception),
                null);

        /// <summary>
        /// Returns a handled result which will return a result object.
        /// </summary>
        /// <param name="result">The result object.</param>
        public static ExceptionHandlingResult Return(object? result)
            => new ExceptionHandlingResult(ExceptionHandling.Return, null, result);

        /// <summary>
        /// Returns a handled result which indicates the exception cannot be handled by current
        /// exception handler. The exception mapper should proceed to other exception handlers.
        /// </summary>
        public static ExceptionHandlingResult NoResult()
            => new ExceptionHandlingResult(ExceptionHandling.NoResult, null, null);

        /// <summary>
        /// Creates an instance of the result.
        /// </summary>
        /// <param name="handling">The exception handling strategy.</param>
        /// <param name="sourceException">The captured exception.</param>
        /// <param name="result">The result object to return when status is set to <see cref="ExceptionHandling.Return"/>.</param>
        public ExceptionHandlingResult(
            ExceptionHandling handling,
            ExceptionDispatchInfo? sourceException,
            object? result)
        {
            Handling = handling;
            IsHandled = true;

            if (Handling == ExceptionHandling.Rethrow && sourceException == null)
            {
                throw new ArgumentNullException(nameof(sourceException));
            }

            _sourceException = sourceException;
            _result = result;
        }

        /// <summary>
        /// Indicates whether the captured exception is handled.
        /// </summary>
        public bool IsHandled { get; }

        /// <summary>
        /// Indicates whether the captured exception is handled successfully
        /// and an handling result has been produced.
        /// </summary>
        public bool IsHandledSuccessfully => IsHandled && Handling != ExceptionHandling.NoResult;

        /// <summary>
        /// Returns strategy of the exception handling.
        /// </summary>
        public ExceptionHandling Handling { get; }

        /// <summary>
        /// Gets the source exception.
        /// </summary>
        public Exception? SourceException => _sourceException?.SourceException;

        /// <summary>
        /// Returns an result object. If <see cref="Handling"/> is set to <see cref="ExceptionHandling.Handled"/>,
        /// this method always returns <c>null</c>. If <see cref="Handling"/> is set to <see cref="ExceptionHandling.Rethrow"/>,
        /// this method re-throws the <see cref="SourceException"/>. If current result is not handled or <see cref="Handling"/>
        /// is set to <see cref="ExceptionHandling.NoResult"/>, this method throws <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When current result is not in a returnable status.</exception>
        public object? GetResult()
        {
            if (IsHandled)
            {
                switch (Handling)
                {
                    case ExceptionHandling.Handled:
                    case ExceptionHandling.Return:
                        return _result;
                    case ExceptionHandling.Rethrow:
                        _sourceException!.Throw();
                        // Should never hit this
                        return null;
                }
            }

            throw new InvalidOperationException("Instance does not contain a returnable result.");
        }

        /// <summary>
        /// Converts the <see cref="ExceptionHandlingResult"/> to a value task which returns the result.
        /// </summary>
        public static implicit operator ValueTask<ExceptionHandlingResult>(in ExceptionHandlingResult result)
        {
            return new ValueTask<ExceptionHandlingResult>(result);
        }
    }
}
