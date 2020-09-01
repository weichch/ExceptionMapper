using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitSharp.Diagnostics;

namespace RabbitSharp.ExceptionMapper.Test.Core.Mapper.Schemes
{
    class ExceptionHandlerWithOptionsOptions : ExceptionMappingSchemeOptions
    {
        public bool Run { get; set; }
    }

    class ExceptionHandlerWithOptions : ExceptionHandler<ExceptionHandlerWithOptionsOptions, object>
    {
        private readonly object _result;

        public ExceptionHandlerWithOptions(
            IOptionsMonitor<ExceptionHandlerWithOptionsOptions> optionsManager,
            IExceptionMappingConventionProvider conventionProvider,
            ILoggerFactory loggerFactory,
            object result)
            : base(optionsManager, conventionProvider, loggerFactory)
        {
            _result = result;
        }

        protected override ValueTask<ExceptionHandlingResult> HandleExceptionAsync(
            ExceptionHandlingContext context)
        {
            if (Options.Run)
            {
                return ExceptionHandlingResult.Return(_result);
            }

            return ExceptionHandlingResult.Skip();
        }
    }
}
