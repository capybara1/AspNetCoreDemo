using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Middleware.Routing
{
    [Trait("Category", "ASP.NET Core Middleware / Routing")]
    public class RoutingDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RoutingDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use simple handler")]
        public async Task UseSimpleHandler()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    var router = new RouteHandler(context =>
                    {
                        return context.Response.WriteAsync("Hello, World!");
                    });
                    app.UseRouter(router);
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/etst?name=World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World!", greeting);
        }

        [Fact(DisplayName = "Use method specific handler with template")]
        public async Task UseMethodSpecificHandlerWithTemplate()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    app.UseRouter(routes =>
                    {
                        // For reserved routing names see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-2.1#reserved-routing-names
                        routes.MapGet("test/{name}", context =>
                        {
                            // The following lines can be simplified by using the GetRouteValue extension method:
                            var routingFeature = (IRoutingFeature)context.Features[typeof(IRoutingFeature)];
                            var routeData = routingFeature.RouteData;
                            var name = routeData.Values["name"];
                            return context.Response.WriteAsync($"Hello, {name}!");
                        });
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test/World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World!", greeting);
        }

        [Fact(DisplayName = "Fork pipeline using method specific handler with template")]
        public async Task ForkPipelineUsingMethodSpecificHandlerWithTemplate()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    app.UseRouter(routes =>
                    {
                        routes.MapMiddlewareGet("test/{name}", fork =>
                        {
                            fork.Run(context =>
                            {
                                var name = context.GetRouteValue("name");
                                return context.Response.WriteAsync($"Hello, {name}!");
                            });
                        });
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test/World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World!", greeting);
        }

        [Fact(DisplayName = "Use plain route template for default handler")]
        public async Task UsePlainRouteTemplateForDefaultHandler()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    var defaultHandler = new RouteHandler(context =>
                    {


                        var name = context.GetRouteValue("name");
                        return context.Response.WriteAsync($"Hello, {name}!");
                    });
                    var routeBuilder = new RouteBuilder(app, defaultHandler);
                    routeBuilder.MapRoute("default", "/test/{name}");
                    var router = routeBuilder.Build();
                    app.UseRouter(router);
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test/World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World!", greeting);
        }

        [Fact(DisplayName = "Use route template and default values for default handler")]
        public async Task UseRouteTemplateAndDefaultValuesForDefaultHandler()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    var defaultHandler = new RouteHandler(context =>
                    {
                        var greeting = context.GetRouteValue("greeting");
                        var name = context.GetRouteValue("name");
                        return context.Response.WriteAsync($"{greeting}, {name}!");
                    });
                    var routeBuilder = new RouteBuilder(app, defaultHandler);
                    routeBuilder.MapRoute("default", "/test/{name}",
                        defaults: new { greeting = "Hello", name = "World" });
                    var router = routeBuilder.Build();
                    app.UseRouter(router);
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World!", content);
        }

        [Fact(DisplayName = "Use route template with optional path component for default handler")]
        public async Task UseRouteTemplateWithOptionalPathComponentForDefaultHandler()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    var defaultHandler = new RouteHandler(context =>
                    {
                        var givenname = context.GetRouteValue("givenname");
                        var familyname = context.GetRouteValue("familyname");
                        return context.Response.WriteAsync($"Hello, {givenname} {familyname}!");
                    });
                    var routeBuilder = new RouteBuilder(app, defaultHandler);
                    routeBuilder.MapRoute("default", "/test/{givenname}/{familyname?}");
                    var router = routeBuilder.Build();
                    app.UseRouter(router);
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test/John");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, John !", greeting);
        }

        [Fact(DisplayName = "Use route template with catch-all path component for default handler")]
        public async Task UseRouteTemplateWithCatchAllPathComponentForDefaultHandler()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    var defaultHandler = new RouteHandler(context =>
                    {
                        var remainder = context.GetRouteValue("remainder");
                        return context.Response.WriteAsync($"Hello, {remainder}!");
                    });
                    var routeBuilder = new RouteBuilder(app, defaultHandler);
                    routeBuilder.MapRoute("default", "/test/{*remainder}");
                    var router = routeBuilder.Build();
                    app.UseRouter(router);
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test/John/Doe");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, John/Doe!", greeting);
        }

        [Fact(DisplayName = "Use route template with constrained path component for default handler")]
        public async Task UseRouteTemplateWithConstrainedPathComponentForDefaultHandler()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    var defaultHandler = new RouteHandler(context =>
                    {
                        var name = context.GetRouteValue("name");
                        return context.Response.WriteAsync($"Hello, {name}!");
                    });
                    var routeBuilder = new RouteBuilder(app, defaultHandler);
                    routeBuilder.MapRoute("default", "/test/{name:alpha}");
                    var router = routeBuilder.Build();
                    app.UseRouter(router);
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test/1");

            Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use route template for url generation")]
        public async Task UseRouteTemplateForUrlGeneration()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    var defaultHandler = new RouteHandler(httpContext =>
                    {
                        var routeValues = new RouteValueDictionary
                        {
                            { "name", "Jane" },
                        };

                        var pathContext = new VirtualPathContext(
                            httpContext,
                            null,
                            routeValues,
                            "default");

                        var routeData = httpContext.GetRouteData();
                        var usedRouter = routeData.Routers.First();

                        var routeContext = new RouteContext(httpContext);
                        var pathData = usedRouter.GetVirtualPath(pathContext);
                        
                        return httpContext.Response.WriteAsync($"Next: {pathData.VirtualPath}");
                    });
                    var routeBuilder = new RouteBuilder(app, defaultHandler);
                    routeBuilder.MapRoute("default", "/test/{name}");
                    var router = routeBuilder.Build();
                    app.UseRouter(router);
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test/John");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Next: /test/Jane", greeting);
        }
    }
}
