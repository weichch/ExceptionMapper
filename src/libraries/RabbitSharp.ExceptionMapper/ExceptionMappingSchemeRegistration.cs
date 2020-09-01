using System;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Represents a registered exception mapping scheme.
    /// </summary>
    public class ExceptionMappingSchemeRegistration
    {
        /// <summary>
        /// Creates an instance of the registered scheme.
        /// </summary>
        /// <param name="name">The name of the scheme.</param>
        /// <param name="handlerType">The handler type.</param>
        /// <param name="handlerFactory">The factory function for creating exception handler.</param>
        public ExceptionMappingSchemeRegistration(
            string name,
            Type handlerType,
            Func<IServiceProvider, IExceptionHandler> handlerFactory)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            if (handlerType.IsAbstract
                || handlerType.IsValueType
                || handlerType.IsGenericTypeDefinition
                || !typeof(IExceptionHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException("Invalid handler type.");
            }

            HandlerType = handlerType;
            HandlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));
        }

        /// <summary>
        /// Gets the name of the scheme.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the handler type.
        /// </summary>
        public Type HandlerType { get; }

        /// <summary>
        /// Gets the factory function for creating exception handler.
        /// </summary>
        public Func<IServiceProvider, IExceptionHandler> HandlerFactory { get; }
    }
}
