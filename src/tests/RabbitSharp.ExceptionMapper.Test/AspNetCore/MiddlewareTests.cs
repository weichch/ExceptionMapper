﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RabbitSharp.Diagnostics.AspNetCore;
using RabbitSharp.Diagnostics.AspNetCore.Conventions;
using RabbitSharp.Diagnostics.Builder;
using Xunit;

namespace RabbitSharp.ExceptionMapper.Test.AspNetCore
{
    public class MiddlewareTests : AppTests
    {
        [Fact]
        public async Task ShouldHandleEndpointException()
        {
            var randomString = Guid.NewGuid().ToString();

            HostBuilder.ConfigureWebHost(webHost =>
            {
                webHost.ConfigureServices(ConfigureServices);
                webHost.Configure(app => Configure(app, randomString));
            });
            var client = await GetClientAsync();

            var response = await client.GetAsync("throw/123");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.Equal($"123:xyz:{randomString}", content);
        }

        [Fact]
        public async Task ShouldHandleControllerException()
        {
            var randomString = Guid.NewGuid().ToString();

            HostBuilder.ConfigureWebHost(webHost =>
            {
                webHost.ConfigureServices(ConfigureServices);
                webHost.Configure(app => Configure(app, randomString));
            });
            var client = await GetClientAsync();

            var response = await client.GetAsync($"test/controller/123/{randomString}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.Equal($"123:xyz:{randomString}", content);
        }

        [Theory]
        [InlineData("throw/problem")]
        [InlineData("test/controller/problem")]
        public async Task ShouldMapToProblemDetails(string url)
        {
            HostBuilder.ConfigureWebHost(webHost =>
            {
                webHost.ConfigureServices(ConfigureServices);
                webHost.Configure(app => Configure(app, ""));
            });
            var client = await GetClientAsync();

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.NotNull(content);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddControllers();

            services.AddExceptionMapping()
                .AddEndpointResponse(scheme =>
                {
                    scheme.DefaultConvention = new DelegateExceptionMappingConvention(
                        (context, httpContext) => Task.CompletedTask);

                    scheme.MapEndpointExceptions(conventions =>
                    {
                        conventions.MapException(_ => true)
                            .UseTags("my-tag", "my-tag3");

                        conventions.MapException<InvalidOperationException>()
                            .ToStatusCode(StatusCodes.Status402PaymentRequired);

                        conventions.MapException<InvalidOperationException>()
                            .ToRequestHandler(async httpContext =>
                            {
                                await Task.Yield();
                            });

                        conventions.MapException<InvalidOperationException>()
                            .ToEndpoint("/error/{value}/{custom}", new {custom = "xyz"})
                            .UseTags("my-tag", "my-tag3");

                        conventions.MapException<InvalidOperationException>()
                            .ToProblemDetails(ctx => ctx.Factory.CreateProblemDetails(
                                ctx.HttpContext, StatusCodes.Status402PaymentRequired))
                            .UseTags("problem-details");
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

                }).MapException("my-tag", "my-tag2").MapException("my-tag3");

                endpoints.MapGet("/throw/problem", async httpContext =>
                {
                    await Task.Yield();
                    throw new InvalidOperationException(expectedMessage);

                }).MapException("problem-details");

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
