using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using RabbitSharp.Diagnostics.AspNetCore;
using RabbitSharp.Diagnostics.AspNetCore.Conventions;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Represents builder of endpoint exception mapping convention.
    /// </summary>
    public class EndpointExceptionMappingConventionBuilder : IExceptionMappingConventionBuilder
    {
        /// <summary>
        /// Gets or sets the order of the mapping convention.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets user-defined tags on the mapping convention.
        /// </summary>
        public IEnumerable<string>? Tags { get; set; }

        /// <summary>
        /// Gets or sets the exception type to map.
        /// </summary>
        public Type? ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets predicate function.
        /// </summary>
        public Func<ExceptionHandlingContext, HttpContext, bool>? Predicate { get; set; }

        /// <summary>
        /// Gets or sets the mapping delegate.
        /// </summary>
        public EndpointExceptionMappingDelegate? MappingDelegate { get; set; }

        /// <summary>
        /// Gets or sets the convention instance.
        /// </summary>
        public IEndpointExceptionMappingConvention? Convention { get; set; }

        /// <summary>
        /// Gets or sets the action to configure conventions.
        /// </summary>
        public Action<ExceptionMappingConventionCollection>? BuildAction { get; set; }

        /// <summary>
        /// Builds the exception mapping convention.
        /// </summary>
        /// <param name="conventions">The target conventions.</param>
        public void Build(ExceptionMappingConventionCollection conventions)
        {
            ValidateBuild();

            if (MappingDelegate != null)
            {
                var convention = DelegateExceptionMappingConvention.CreateConditional(
                    MappingDelegate, ExceptionType, Predicate);
                conventions.AddConvention(convention, Tags, Order);
            }
            else if (Convention != null)
            {
                conventions.AddConvention(Convention, Tags, Order);
            }
            else
            {
                BuildAction?.Invoke(conventions);
            }
        }

        /// <summary>
        /// Validates the builder and throws if the builder has conflicts.
        /// </summary>
        private void ValidateBuild()
        {
            var actualExceptionType = ExceptionType ?? typeof(Exception);

            if (actualExceptionType != typeof(Exception) || Predicate != null)
            {
                // Convention is conditional by exception type and predicate, must have mapping delegate.
                if (MappingDelegate == null)
                {
                    throw new InvalidOperationException(
                        "Mapping delegate must be set when either exception type or predicate function provided.");
                }

                return;
            }

            if (MappingDelegate == null 
                && Convention == null 
                && BuildAction == null)
            {
                throw new InvalidOperationException("No build method provided.");
            }
        }
    }
}
