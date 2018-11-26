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

namespace AspNetCoreDemo.Mvc.Basic
{
    [Trait("Category", "ASP.NET Core MVC / Antiforgery")]
    public class AntiforgeryDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AntiforgeryDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }
        
        [Fact(DisplayName = "Use ..")]
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                        .AddAntiforgery;
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
    }
    }
