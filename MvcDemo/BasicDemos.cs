using AspNetCoreDemo.Utils;
using AspNetCoreDemo.Utils.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
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

        // Convention based routing uses the ASP.NET Core route builder
        // to provide routes that contain the reserved route value names 'area', 
        // 'controller',and 'action'. The latter are required to resolve
        // a controller action; the first on is optional. The route value
        // with name 'controller' refers to the name of a controller class
        // without the suffix 'Controller'. 

        [Fact(DisplayName = "Use controller with convention based routing (custom route)")]
        public async Task UseControllerWithConventionBasedRouting()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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

        [Fact(DisplayName = "Use controller with convention based routing (MVC default route)")]
        public async Task UseControllerWithConventionBasedRoutingAndMvcDefaultRoute()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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

        // Attribute routing relies on the existence of a Route-attribute on a controller class.
        // Controller action may also define a part of the final route by using a Route-attribute
        // or the constructor overloads of the Http[Verb]-attributes.

        [Fact(DisplayName = "Use Controller with attribute routing to action with multiple methods")]
        public async Task UseControllerWithAttributeRoutingToActionWithMultipleMethods()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.PutAsync("/explicit/test1", null);
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, Explicit World!", greeting);
        }

        [Fact(DisplayName = "Use Controller with attribute routing to action with multiple methods")]
        public async Task UseControllerWithAttributeRoutingToActionWithMultipleRoutes()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/explicit/test2/first");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, Explicit World!", greeting);
        }

        [Fact(DisplayName = "Use Controller with attribute routing to action with conflicting unordered routes")]
        public async Task UseControllerWithAttributeRoutingToActionWithConflictingUnorderedRoutes()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/explicit/test3/morespecific");
            var methodName = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal(nameof(Controllers.AttributeRoutingExamplesController.SecondActionWithConflictingUnorderedRoutes), methodName);
        }

        [Fact(DisplayName = "Use Controller with attribute routing to action with conflicting unordered routes")]
        public async Task UseControllerWithAttributeRoutingToActionWithConflictingOrderedRoutes()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/explicit/test4/morespecific");
            var methodName = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal(nameof(Controllers.AttributeRoutingExamplesController.SecondActionWithConflictingOrderedRoutes), methodName);
        }

        [Fact(DisplayName = "Use Controller with attribute routing to action with conflicting unordered routes")]
        public async Task UseControllerWithAttributeRoutingToActionWithConstrainedParameterInRoute()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/explicit/test5/1234");
            var methodName = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal(nameof(Controllers.AttributeRoutingExamplesController.FirstActionWithConstrainedParameterInRoute), methodName);
        }

        [Fact(DisplayName = "Use Controller with attribute routing to action with catch all")]
        public async Task UseControllerWithAttributeRoutingToActionWithCatchAll()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/explicit/test6/some/more/parameters");
            var methodName = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal(nameof(Controllers.AttributeRoutingExamplesController.ActionWithCatchAll), methodName);
        }

        [Fact(DisplayName = "Use Controller with attribute routing to ambiguous actions")]
        public async Task UseControllerWithAttributeRoutingToAmbiguousActions()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            await Assert.ThrowsAnyAsync<AmbiguousActionException>(async () => await client.GetAsync("/explicit/test7"));
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
                    services.AddMvcCore()
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                })
                .Configure(app =>
                {
                    app.UseMvc(routes =>
                    {
                        routes.MapRoute("default", "/",
                            new
                            {
                                controller = nameof(Controllers.TestController).RemoveControllerSuffix(),
                                action = nameof(Controllers.TestController.NoResultAction),
                            });
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.PutAsync("/", null);

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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                        .AddJsonFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvc(routes =>
                    {
                        routes.MapRoute("default", "/",
                            new
                            {
                                controller = nameof(Controllers.TestController).RemoveControllerSuffix(),
                                action = nameof(Controllers.TestController.ModelResultAction),
                            });
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.PutAsync("/", null);
            var value = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("{\"priority\":0,\"text\":\"my example\"}", value);
        }
    }
}
