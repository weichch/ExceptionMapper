using System;
using Microsoft.AspNetCore.Http;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods for building mapping conventions.
    /// </summary>
    public static class EndpointExceptionMappingConventionsBuilderExtensions
    {
        /// <summary>
        /// TODO: Internal for now until implemented. Maps exception to an endpoint exception mapping convention.
        /// </summary>
        /// <param name="builder">The conventions builder.</param>
        internal static EndpointExceptionMappingConventionBuilder MapException(
            this EndpointExceptionMappingConventionsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var mappingBuilder = new EndpointExceptionMappingConventionBuilder();
            builder.Add(mappingBuilder.Build);

            return mappingBuilder;
        }

        /// <summary>
        /// Maps exception to an endpoint exception mapping convention when exception
        /// mapping context meets criteria.
        /// </summary>
        /// <param name="builder">The conventions builder.</param>
        /// <param name="predicate">The function to determine whether exception mapping context meets criteria.</param>
        public static EndpointExceptionMappingConventionBuilder MapException(
            this EndpointExceptionMappingConventionsBuilder builder,
            Func<ExceptionHandlingContext, bool> predicate)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var mappingBuilder = new EndpointExceptionMappingConventionBuilder
            {
                Predicate = (context, _) => predicate(context)
            };
            builder.Add(mappingBuilder.Build);

            return mappingBuilder;
        }

        /// <summary>
        /// Maps exception to an endpoint exception mapping convention when exception
        /// mapping context meets criteria.
        /// </summary>
        /// <param name="builder">The conventions builder.</param>
        /// <param name="predicate">The function to determine whether exception mapping context meets criteria.</param>
        public static EndpointExceptionMappingConventionBuilder MapException(
            this EndpointExceptionMappingConventionsBuilder builder,
            Func<ExceptionHandlingContext, HttpContext, bool> predicate)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var mappingBuilder = new EndpointExceptionMappingConventionBuilder
            {
                Predicate = predicate
            };
            builder.Add(mappingBuilder.Build);

            return mappingBuilder;
        }

        /// <summary>
        /// Maps exception to an endpoint exception mapping convention when type of captured exception
        /// is <typeparamref name="TException"/>.
        /// </summary>
        /// <typeparam name="TException">The type of exception to map.</typeparam>
        /// <param name="builder">The conventions builder.</param>
        public static EndpointExceptionMappingConventionBuilder MapException<TException>(
            this EndpointExceptionMappingConventionsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var mappingBuilder = new EndpointExceptionMappingConventionBuilder
            {
                ExceptionType = typeof(TException)
            };
            builder.Add(mappingBuilder.Build);

            return mappingBuilder;
        }
    }
}
