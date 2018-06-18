using AspNetCoreDemo.MiddlewareDemo.Contracts;
using AspNetCoreDemo.MiddlewareDemo.Middlewares;
using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.MiddlewareDemo
{
    // Note: this serves a demonstration purpose and may be used as a transition to MVC

    [Trait("Category", "ASP.NET Core Middleware / RESTful API")]
    public class RestfulApiDemo
    {
        private readonly Mock<IResourceStore> ResourceStoreMock = new Mock<IResourceStore>();
        private readonly Mock<ISerializer> SerializerMock = new Mock<ISerializer>();

        private readonly ITestOutputHelper _testOutputHelper;

        public RestfulApiDemo(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Non existing resource")]
        public async Task RestfulGet()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddTransient<RestfulApiMiddleware>();
                    serviceCollection.AddSingleton(ResourceStoreMock.Object);
                    serviceCollection.AddSingleton(SerializerMock.Object);
                })
                .Configure(app =>
                {
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<RestfulApiDemo>>();
                    var store = app.ApplicationServices.GetRequiredService<ILogger<RestfulApiDemo>>();

                    app.UseExceptionHandler(new ExceptionHandlerOptions
                    {
                        ExceptionHandler = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            return Task.CompletedTask;
                        }
                    });
                    app.UseMiddleware<RestfulApiMiddleware>();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/notexisting");
            Assert.Equal(404, (int)response.StatusCode);
        }
    }
}
