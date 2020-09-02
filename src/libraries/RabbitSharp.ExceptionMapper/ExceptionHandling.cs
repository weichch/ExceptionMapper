namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Represents strategy of exception handling.
    /// </summary>
    public enum ExceptionHandling
    {
        /// <summary>
        /// Indicates the source exception should be re-thrown.
        /// </summary>
        Rethrow = 0,

        /// <summary>
        /// Indicates the source exception is handled and no further exception handling needed.
        /// </summary>
        Handled = 1,

        /// <summary>
        /// Indicates the exception mapper should return a specified result object.
        /// </summary>
        Return = 2,
    }
}
