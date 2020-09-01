using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RabbitSharp.ExceptionMapper.Test.AspNetCore
{
    public abstract class AppTests
    {
        private IHost? _host;
        private HttpClient? _client;

        protected AppTests()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureWebHost(webHost =>
            {
                webHost.ConfigureServices(serviceCollection =>
                    serviceCollection
                        .AddHttpContextAccessor()
                        .AddRouting()
                        .AddExceptionMapping());
                webHost.UseTestServer();
            });

            HostBuilder = hostBuilder;
        }

        protected IHostBuilder HostBuilder { get; }

        protected IHost Host => _host ??= HostBuilder.Build();

        public void Dispose()
        {
            _client?.Dispose();
            _host?.Dispose();
        }

        protected async Task<HttpClient> GetClientAsync()
        {
            if (_client == null)
            {
                await Host.StartAsync();
                _client = _host.GetTestClient();
            }

            return _client;
        }
    }
}
