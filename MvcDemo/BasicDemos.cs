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

namespace AspNetCoreDemo.MvcDemo
{
    [Trait("Category", "ASP.NET Core MVC / Basics")]
    public class BasicDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BasicDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }
        
        [Fact(DisplayName = "Use controller with convention based routing (default route)")]
        public async Task UseControllerWithConventionBasedRoutingAndDefaultRoute()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore();
                })
                .Configure(app =>
                {
                    app.UseMvc(routes =>
                    {
                        routes.MapRoute(
                            name: "default",
                            template: "api/{controller=ConventionBasedRoutingExamples}/{action=Get}/{id?}");
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/api");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World!", greeting);
        }
        
        [Fact(DisplayName = "Use Controller with Non-Default Routes (non-default route)")]
        public async Task UseControllerWithConventionBasedRoutingAndNonDefaultRoute()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore();
                })
                .Configure(app =>
                {
                    // This sets up the default route with a template {controller=Home}/{action=Index}/{id?}
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/ConventionBasedRoutingExamples/Get");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World!", greeting);
        }

        [Fact(DisplayName = "Use Controller with explicit routes")]
        public async Task UseControllerWithExplicitRoutes()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore();
                })
                .Configure(app =>
                {
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            
            var response = await client.PutAsync("/explicit/test", null);
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, Explicit World!", greeting);
        }

        [Fact(DisplayName = "Use Controller Action with no return value")]
        public async Task UseControllerActionWithNoReturnValue()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore();
                })
                .Configure(app =>
                {
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.PutAsync("/test/noResultAction", null);

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }
        
        [Fact(DisplayName = "Use Controller Action with model return value")]
        public async Task UseControllerActionWithModelReturnValue()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore()
                        .AddJsonFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.PutAsync("/test/modelResultAction", null);
            var value = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("{\"priority\":0,\"text\":\"my example\"}", value);
        }
        
        [Fact(DisplayName = "Use Controller Action with CreatedResponse")]
        public async Task UseControllerActionWithCreatedResponse()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore()
                        .AddJsonFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.PutAsync("/test/modelResultAction", null);
            var value = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("{\"priority\":0,\"text\":\"my example\"}", value);
        }
    }
}
