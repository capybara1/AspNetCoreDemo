﻿using AspNetCoreDemo.MvcDemo.Filter;
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
    [Trait("Category", "ASP.NET Core MVC / Filters")]
    public class FilterDemo
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FilterDemo(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Demo of Filters with Action That Throws")]
        public async Task RequestActionThatThrows()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore(setup =>
                    {
                        setup.Filters.Add<ExceptionFilter>();
                    });
                })
                .Configure(app =>
                {
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test/failingAction");

            Assert.Equal(StatusCodes.Status500InternalServerError, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Demo of Filters with Dependency Injection")]
        public async Task UseFilterWithDependencyInjection()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddMvcCore(setup =>
                    {
                        setup.Filters.Add<ExceptionFilter>();
                    });
                })
                .Configure(app =>
                {
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/test/failingAction");

            Assert.Equal(StatusCodes.Status500InternalServerError, (int)response.StatusCode);
        }
    }
}
