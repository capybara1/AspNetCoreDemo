using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.MiddlewareDemo
{
    [Trait("Category", "ASP.NET Core Middleware / Hosted Servicer")]
    public partial class HostedServiceDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public HostedServiceDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Setup hosted example service")]
        public async Task SetupHostedExampleService()
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Services.HostedExampleService>();
                })
                .Configure(app =>
                {
                    app.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync($"Hello, World!", new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(webHostBuilder);
            using (server)
            {
                await Task.Delay(20);
            }
        }
    }
}
