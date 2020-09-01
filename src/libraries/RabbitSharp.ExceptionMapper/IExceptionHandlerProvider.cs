using System.Threading.Tasks;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Defines a type which provisions <see cref="IExceptionHandler"/> for a scheme.
    /// </summary>
    public interface IExceptionHandlerProvider
    {
        /// <summary>
        /// Prepares an instance of <see cref="IExceptionHandler"/> for specified exception
        /// mapping scheme.
        /// </summary>
        /// <param name="scheme">The exception mapping scheme.</param>
        Task<IExceptionHandler> GetHandlerAsync(ExceptionMappingSchemeRegistration scheme);
    }
}
