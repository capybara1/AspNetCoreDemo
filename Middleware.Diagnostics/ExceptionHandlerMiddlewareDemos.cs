using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Middleware.Diagnostics
{
    [Trait("Category", "ASP.NET Core Middleware / Exception Handler Middleware")]
    public class ExceptionHandlerMiddlewareDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ExceptionHandlerMiddlewareDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "ExceptionHandler Middleware Demo")]
        public async Task ExceptionHandlerMiddlewareDemo()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .Configure(app =>
                {
                    app.UseExceptionHandler(new ExceptionHandlerOptions
                    {
                        ExceptionHandler = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            return Task.CompletedTask;
                        }
                    });
                    app.Run(context =>
                    {
                        return Task.FromException(new Exception("Unexpected error"));
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/");

            Assert.Equal(StatusCodes.Status500InternalServerError, (int)response.StatusCode);
        }
    }
}
