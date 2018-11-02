using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Mvc.Versioning
{
    [Trait("Category", "ASP.NET Core MVC / Versioning")]
    public class BasicDemos
    {
        // See https://github.com/Microsoft/aspnet-api-versioning
        
        private readonly ITestOutputHelper _testOutputHelper;

        public BasicDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "URL segment based versioning")]
        public async Task UrlSegmentBasedVersioning()
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
                    services.AddApiVersioning(setup => setup.ApiVersionReader = new UrlSegmentApiVersionReader());
                })
                .Configure(app =>
                {
                    app.UseMvc(); // Relies on attribute routing
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/v2.0/versioning");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World 2.0!", greeting);
        }

        [Fact(DisplayName = "Header based versioning")]
        public async Task HeaderBasedVerisoning()
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
                    services.AddApiVersioning(setup => setup.ApiVersionReader = new HeaderApiVersionReader("Api-Version"));
                })
                .Configure(app =>
                {
                    app.UseMvc(); // Relies on attribute routing
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Api-Version", "2.0");

            var response = await client.GetAsync("/versioning");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World 2.0!", greeting);
        }

        [Fact(DisplayName = "Media-type based versioning")]
        public async Task MediaTypeBasedVersioning()
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
                    services.AddApiVersioning(setup => setup.ApiVersionReader = new MediaTypeApiVersionReader());
                })
                .Configure(app =>
                {
                    app.UseMvc(); // Relies on attribute routing
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json;v=2.0");

            var response = await client.GetAsync("/versioning");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World 2.0!", greeting);
        }

        [Fact(DisplayName = "Query string based versioning")]
        public async Task QueryStringBasedVersioning()
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
                    services.AddApiVersioning(setup => setup.ApiVersionReader = new QueryStringApiVersionReader("v"));
                })
                .Configure(app =>
                {
                    app.UseMvc(); // Relies on attribute routing
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("versioning/?v=2.0");
            var greeting = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("Hello, World 2.0!", greeting);
        }
    }
}
