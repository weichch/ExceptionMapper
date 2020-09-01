using System;
using System.Threading.Tasks;
using RabbitSharp.Diagnostics;

namespace RabbitSharp.ExceptionMapper.Test.Core.Mapper.Schemes
{
    class ReturnResultExceptionHandler : IExceptionHandler
    {
        private readonly object _result;

        public ReturnResultExceptionHandler(object result)
        {
            _result = result;
        }

        public ValueTask<ExceptionHandlingResult> HandleExceptionAsync(
            Exception exception, 
            ExceptionMappingContext mappingContext)
        {
            return ExceptionHandlingResult.Return(_result);
        }

        public ValueTask InitializeAsync(string name)
        {
            return default;
        }
    }
}
