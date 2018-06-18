using AspNetCoreDemo.MiddlewareDemo.Contracts;
using AspNetCoreDemo.MiddlewareDemo.Implementations;
using AspNetCoreDemo.MiddlewareDemo.Middlewares;
using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.MiddlewareDemo
{
    [Trait("Category", "ASP.NET Core Middleware / Basics")]
    public partial class BasicDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BasicDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Run Single Middleware")]
        public async Task RunSingleMiddlewareAsync()
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .Configure(app =>
                {
                    app.Run(async context =>
                    {
                        var path = context.Request.Path;
                        var name = Regex.Match(path, @"(?<=^\/hello\/).*$").Value;

                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync($"Hello, {name}!", new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(webHostBuilder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/hello/World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal("Hello, World!", greeting);
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Run Chained Middleware")]
        public async Task RunChainedMiddlewareAsync()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .Configure(app =>
                {
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<BasicDemos>>();

                    app.Use(async (context, next) =>
                    {
                        logger.LogInformation("Middleware 1 Pre");
                        await next();
                        logger.LogInformation("Middleware 1 Post");
                    });

                    app.Use(async (context, next) =>
                    {
                        logger.LogInformation("Middleware 2 Pre");
                        if (context.Request.Headers["MyHeader"].FirstOrDefault() != "ticket")
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }
                        await next();
                        logger.LogInformation("Middleware 2 Post");
                    });

                    app.Run(async context =>
                    {
                        var path = context.Request.Path;
                        var name = Regex.Match(path, @"(?<=^\/hello\/).*$").Value;

                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync($"Hello, {name}!", new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/hello/World");

            Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Run Mapped Middleware")]
        public async Task RunMappedMiddlewareAsync()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .Configure(app =>
                {
                    app.Map("/test1", mappedApp =>
                    {
                        mappedApp.Run(async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync("First route", new UTF8Encoding(false));
                        });
                    });

                    app.Map("/test2", mappedApp =>
                    {
                        mappedApp.Run(async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync("Second route", new UTF8Encoding(false));
                        });
                    });

                    app.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("Unmapped route", new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test2");
            var greeting = await response.Content.ReadAsStringAsync();
            Assert.Equal("Second route", greeting);
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Run Single Middleware Class")]
        public async Task RunMiddlewareClassAsync()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddTransient<ExampleMiddleware>();
                })
                .Configure(app =>
                {
                    app.UseMiddleware<ExampleMiddleware>();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/hello/World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal("Hello, World!", greeting);
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Run Single Middleware Class with Dependency")]
        public async Task RunMiddlewareClassWithDependencyAsync()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddTransient<IExampleService, ServiceImplementationA>();
                    services.AddTransient<ExampleMiddlewareWithDependency>();
                })
                .Configure(app =>
                {
                    app.UseMiddleware<ExampleMiddlewareWithDependency>();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/hello/World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal("Hello, World!", greeting);
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Run Single Middleware Class using Extension Methods")]
        public async Task RunMiddlewareClassUsingExtensionMethodsAsync()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddExample();
                })
                .Configure(app =>
                {
                    app.UseExample();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/hello/World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal("Hello, World!", greeting);
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Run Single Middleware Class using Startup Class")]
        public async Task RunMiddlewareClassUsingStartupClassAsync()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup
                        .AddDebug()
                        .SetupDemoLogging(_testOutputHelper);
                })
                .UseStartup<Startup>();

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/hello/World");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal("Hello, World!", greeting);
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }
        
        [Fact(DisplayName = "Modify Response after transmittion started")]
        public async Task ModifyResponseAfterTransmissionStarted()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup
                        .AddDebug()
                        .SetupDemoLogging(_testOutputHelper);
                })
                .Configure(app =>
                {
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<BasicDemos>>();

                    app
                        .Use(async (context, next) =>
                        {
                            logger.LogInformation("Middleware 1 Pre");
                            logger.LogInformation($"Transmission has started: {context.Response.HasStarted}");
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync("Hello, World!", new UTF8Encoding(false));
                            logger.LogInformation($"Transmission has started: {context.Response.HasStarted}");
                            await next();
                            logger.LogInformation("Middleware 1 Post");
                        })
                        .Run(context =>
                        {
                            logger.LogInformation("Middleware 2");
                            context.Response.StatusCode = StatusCodes.Status204NoContent;
                            return Task.CompletedTask;
                        });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            await Assert.ThrowsAsync<Exception>(async () => await client.GetAsync("/"));
        }
    }
}
