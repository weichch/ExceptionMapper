using System;
using System.Collections.Generic;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Defines an exception mapping convention which filters conventions by tags based on exception type.
    /// If <see cref="MapExceptionAttribute"/> is present and <see cref="MapExceptionAttribute.Tags"/> is not empty,
    /// the values in <see cref="Tags"/> must be a subset of <see cref="MapExceptionAttribute.Tags"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ExceptionMappingAttribute : Attribute, IExceptionMappingMetadata
    {
        /// <summary>
        /// Creates an instance of the exception mapping convention.
        /// </summary>
        /// <param name="exceptionType">The type of the exception.</param>
        /// <param name="tag">The tag used to locate the destination mapping convention.</param>
        /// <param name="otherTags">The other tags.</param>
        public ExceptionMappingAttribute(Type exceptionType, string tag, params string[] otherTags)
        {
            ExceptionType = exceptionType ?? throw new ArgumentNullException(nameof(exceptionType));
            Tags = new HashSet<string>(StringComparer.Ordinal) {tag};

            if (otherTags != null)
            {
                foreach (var another in otherTags)
                {
                    Tags.Add(another);
                }
            }
        }

        /// <summary>
        /// Gets type of target exception.
        /// </summary>
        public Type ExceptionType { get; }

        /// <summary>
        /// Gets a collection of tags for filtering exception mapping conventions.
        /// </summary>
        public ISet<string> Tags { get; }
    }
}
