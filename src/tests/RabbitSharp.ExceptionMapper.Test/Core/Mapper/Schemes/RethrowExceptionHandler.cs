using System;
using System.Threading.Tasks;
using RabbitSharp.Diagnostics;

namespace RabbitSharp.ExceptionMapper.Test.Core.Mapper.Schemes
{
    class RethrowExceptionHandler : IExceptionHandler
    {
        public ValueTask<ExceptionHandlingResult> HandleExceptionAsync(
            Exception exception,
            ExceptionMappingContext mappingContext)
        {
            return ExceptionHandlingResult.Rethrow(exception);
        }

        public ValueTask InitializeAsync(string name)
        {
            return default;
        }
    }
}
