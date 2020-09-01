using System;
using Microsoft.Extensions.DependencyInjection;
using RabbitSharp.Diagnostics;
using RabbitSharp.Diagnostics.Builder;

namespace RabbitSharp.ExceptionMapper.Test.Core
{
    static class MapperTestHelper
    {
        public static IExceptionMapper CreateMapper(
            Action<ExceptionMapperOptions>? configure = null,
            Action<ExceptionMappingBuilder>? buildAction = null)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            var builder = configure == null
                ? serviceCollection.AddExceptionMapping()
                : serviceCollection.AddExceptionMapping(configure);
            buildAction?.Invoke(builder);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IExceptionMapper>();

            return mapper;
        }
    }
}
