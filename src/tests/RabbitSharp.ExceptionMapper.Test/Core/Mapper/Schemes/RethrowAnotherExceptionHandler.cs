using System;
using System.Threading.Tasks;
using RabbitSharp.Diagnostics;

namespace RabbitSharp.ExceptionMapper.Test.Core.Mapper.Schemes
{
    class RethrowAnotherExceptionHandler : IExceptionHandler
    {
        public ValueTask<ExceptionHandlingResult> HandleExceptionAsync(
            Exception exception, 
            ExceptionMappingContext mappingContext)
        {
            return ExceptionHandlingResult.Rethrow(new AggregateException(exception));
        }

        public ValueTask InitializeAsync(string name)
        {
            return default;
        }
    }
}
