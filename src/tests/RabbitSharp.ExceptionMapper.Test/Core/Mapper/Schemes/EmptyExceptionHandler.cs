using System;
using System.Threading.Tasks;
using RabbitSharp.Diagnostics;

namespace RabbitSharp.ExceptionMapper.Test.Core.Mapper.Schemes
{
    class EmptyExceptionHandler : IExceptionHandler
    {
        public ValueTask<ExceptionHandlingResult> HandleExceptionAsync(
            Exception exception, 
            ExceptionMappingContext mappingContext)
        {
            return default;
        }

        public ValueTask InitializeAsync(string name)
        {
            return default;
        }
    }
}
