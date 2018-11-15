using AspNetCoreDemo.ExternalTestLibrary;
using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Middleware.Hosting
{
    [Trait("Category", "ASP.NET Core Middleware / External Assemblies")]
    public partial class ExternalAssembliesDemos : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ExternalAssembliesDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Include startup of external assembly")]
        public async Task IncludeStartupOfExternalAssembly()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await PerformTest());

            Environment.SetEnvironmentVariable(
                "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES",
                "AspNetCoreDemo.ExternalTestLibrary");

            await PerformTest();
        }

        private async Task PerformTest()
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .Configure(app =>
                {
                    var externalService = app.ApplicationServices.GetRequiredService<IExternalService>();

                    app.Run(async context =>
                    {
                        var text = externalService.GetResponseText();
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(text, new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(webHostBuilder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/hello/World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal("Hello from external service", greeting);
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(
                "ASPNETCORE_HOSTINGSTARTUPASSEMBLIES",
                string.Empty);
        }
    }
}
