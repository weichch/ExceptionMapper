using System;
using System.Threading.Tasks;
using RabbitSharp.Diagnostics;
using Xunit;

namespace RabbitSharp.ExceptionMapper.Test.Core.Mapper
{
    public class BasicTests
    {
        [Fact]
        public async Task ShouldRethrowByDefault()
        {
            var mapper = MapperTestHelper.CreateMapper();

            await Assert.ThrowsAsync<Exception>(async () =>
                await mapper.MapExceptionAsync(new Exception()));
        }

        [Fact]
        public async Task ShouldReturnNullIfSuppressDefault()
        {
            var mapper = MapperTestHelper.CreateMapper(opt =>
                opt.FallbackExceptionHandler = async ctx =>
                {
                    await Task.Yield();
                    ctx.Result = ExceptionHandlingResult.Return(null);
                });

            var result = await mapper.MapExceptionAsync(new Exception());

            Assert.Null(result);
        }
    }
}
