using System;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Indicates the class or method where this attribute is applied should be ignored
    /// by exception mapper.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ExcludeFromExceptionMappingAttribute : Attribute, IExcludeFromExceptionMapping
    {
    }
}
