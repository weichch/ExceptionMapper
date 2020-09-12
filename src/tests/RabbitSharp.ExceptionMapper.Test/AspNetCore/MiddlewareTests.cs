using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitSharp.Diagnostics.AspNetCore;
using RabbitSharp.Diagnostics.Builder;
using Xunit;

namespace RabbitSharp.ExceptionMapper.Test.AspNetCore
{
    public class MiddlewareTests : AppTests
    {
        [Fact]
        public async Task ShouldHandleException()
        {
            var randomString = Guid.NewGuid().ToString();

            HostBuilder.ConfigureWebHost(webHost =>
            {
                webHost.ConfigureServices(ConfigureServices);
                webHost.Configure(app => Configure(app, randomString));
            });
            var client = await GetClientAsync();

            var response = await client.GetAsync("/throw/123");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.Equal($"123:xyz:{randomString}", content);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddExceptionMapping()
                .AddEndpointResponse(scheme =>
                {
                    //scheme.FallbackErrorResponseFactory;

                    scheme.MapEndpointExceptions(conventions =>
                    {
                        conventions.MapException<InvalidOperationException>()
                            .ToStatusCode(StatusCodes.Status402PaymentRequired);

                        conventions.MapException<InvalidOperationException>()
                            .ToEndpoint("/error/{value}/{custom}", new {custom = "xyz"})
                            .UseTags("my-tag");

                        conventions.MapException<InvalidOperationException>()
                            .ToRequestHandler(async httpContext =>
                            {
                                await Task.Yield();
                            });
                    });
                });
        }

        private static void Configure(IApplicationBuilder app, string expectedMessage)
        {
            app.UseEndpointExceptionMapping();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/throw/{value:int}", async httpContext =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException(expectedMessage);

                }).MapException("my-tag");

                endpoints.MapGet("/error/{value:int}/{custom}", async httpContext =>
                {
                    var feature = httpContext.Features.Get<IExceptionMappingContextFeature>();
                    var routeValues = httpContext.Features.Get<IRouteValuesFeature>().RouteValues;

                    httpContext.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                    await httpContext.Response.WriteAsync($"{routeValues["value"]}:{routeValues["custom"]}:");
                    await httpContext.Response.WriteAsync(feature.Context!.Exception.Message);

                }).ExcludeFromExceptionMapping();

                endpoints.MapControllers();
            });
        }
    }
}
