using System;
using System.Threading.Tasks;
using RabbitSharp.Diagnostics;
using RabbitSharp.Diagnostics.Builder;
using RabbitSharp.ExceptionMapper.Test.Core.Mapper.Schemes;
using Xunit;

namespace RabbitSharp.ExceptionMapper.Test.Core.Mapper
{
    public class SchemeTests
    {
        [Fact]
        public async Task ShouldIgnoreUnhandledSchemeResult()
        {
            var mapper = MapperTestHelper.CreateMapper(
                buildAction: builder =>
                    builder.AddScheme<EmptyExceptionHandler>("empty"));

            await Assert.ThrowsAsync<Exception>(async () =>
                await mapper.MapExceptionAsync(new Exception()));
        }

        [Fact]
        public async Task ShouldRethrowSchemeResult()
        {
            var mapper = MapperTestHelper.CreateMapper(buildAction: builder =>
                builder.AddScheme<RethrowExceptionHandler>("rethrow"));

            await Assert.ThrowsAsync<Exception>(async () =>
                await mapper.MapExceptionAsync(new Exception()));
        }

        [Fact]
        public async Task ShouldRethrowEncapsulatedSchemeResult()
        {
            var mapper = MapperTestHelper.CreateMapper(buildAction: builder =>
                builder.AddScheme<RethrowAnotherExceptionHandler>("rethrow"));

            await Assert.ThrowsAsync<AggregateException>(async () =>
                await mapper.MapExceptionAsync(new Exception()));
        }

        [Fact]
        public async Task ShouldReturnSchemeResult()
        {
            var result = new object();
            var mapper = MapperTestHelper.CreateMapper(buildAction: builder =>
                builder.AddParameterizedScheme<ReturnResultExceptionHandler>("return", result));

            var actual = await mapper.MapExceptionAsync(new Exception());

            Assert.Equal(result, actual);
        }

        [Fact]
        public async Task ShouldReturnSchemeResultAsPerOptions()
        {
            var result = new object();
            var mapper = MapperTestHelper.CreateMapper(buildAction: builder => builder
                .AddParameterizedScheme<ExceptionHandlerWithOptionsOptions, object, ExceptionHandlerWithOptions>(
                    "return1", opt => opt.Run = false, new object())
                .AddParameterizedScheme<ExceptionHandlerWithOptionsOptions, object, ExceptionHandlerWithOptions>(
                    "return2", opt => opt.Run = true, result)
                .AddParameterizedScheme<ExceptionHandlerWithOptionsOptions, object, ExceptionHandlerWithOptions>(
                    "return3", opt => opt.Run = true, new object()));

            var actual = await mapper.MapExceptionAsync(new Exception());

            Assert.Equal(result, actual);
        }
    }
}
