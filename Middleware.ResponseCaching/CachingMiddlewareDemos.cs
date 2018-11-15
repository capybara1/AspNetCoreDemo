using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Middleware.ResponseCaching
{
    [Trait("Category", "ASP.NET Core Middleware / Caching Middleware")]
    public class CachingMiddlewareDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CachingMiddlewareDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "In-Memory Caching")]
        public async Task InMemoryCaching()
        {
            var builder = new WebHostBuilder()
            .ConfigureLogging(setup =>
            {
                setup.AddDebug();
                setup.SetupDemoLogging(_testOutputHelper);
            })
            .ConfigureServices(services =>
            {
                services
                    .AddMemoryCache(); // Requires Microsoft.Extensions.Caching.Memory package
            })
            .Configure(app =>
            {
                app.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Hello, World!", new UTF8Encoding(false));
                });
            });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("be/complex_parameter?example.text=example%20text&example.priority=2");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Response Caching")]
        public async Task ResponseCaching()
        {
            var builder = new WebHostBuilder()
            .ConfigureLogging(setup =>
            {
                setup
                    .AddDebug()
                    .SetupDemoLogging(_testOutputHelper);
            })
            .ConfigureServices(services =>
            {
                services
                    .AddLogging()
                    .AddResponseCaching(); // Requires Microsoft.AspNetCore.ResponseCaching package
            })
            .Configure(app =>
            {
                var logger = app.ApplicationServices.GetRequiredService<ILogger<CachingMiddlewareDemos>>();

                app.UseResponseCaching();
                app.Run(async context =>
                {
                    logger.LogInformation("Executing Middleware");

                    var headers = context.Response.GetTypedHeaders();
                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(5),
                    };
                    context.Response.Headers[HeaderNames.Vary] = new string[] { "Accept-Encoding" };

                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Hello, World!", new UTF8Encoding(false));
                });
            });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);

            var otherResponse = await client.GetAsync("/");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

    }
}
