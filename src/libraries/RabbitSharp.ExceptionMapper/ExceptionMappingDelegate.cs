using System.Threading.Tasks;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Defines a delegate function which converts a captured exception to an <see cref="ExceptionHandlingResult"/>.
    /// </summary>
    /// <param name="context">The data context for exception handler.</param>
    public delegate ValueTask ExceptionMappingDelegate(ExceptionHandlingContext context);
}
