using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Mvc.Basic
{
    [Trait("Category", "ASP.NET Core MVC / API-Controller")]
    public class ApiControllerDemos
    {
        // See also https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-2.1

        private readonly ITestOutputHelper _testOutputHelper;

        public ApiControllerDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Suppress specific behaviors of API-Controller")]
        public async Task SuppressSpecificBehaviorsOfApiController()
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
                    services.Configure<ApiBehaviorOptions>(options =>
                    {
                        // Available but not used in this example
                        // options.SuppressConsumesConstraintForFormFileParameters
                        // options.SuppressInferBindingSourcesForParameters

                        options.SuppressModelStateInvalidFilter = true;
                    });

                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/apicontroller/invalid");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use API-Controller to perform model validation")]
        public async Task UseApiControllerToPerformModelValidation()
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
                    services.Configure<ApiBehaviorOptions>(options =>
                    {
                        options.InvalidModelStateResponseFactory = context =>
                        {
                            return new BadRequestObjectResult(context.ModelState);
                        };
                    });
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/apicontroller/invalid");

            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use API-Controller to infer parameter sources")]
        public async Task UseApiControllerToInferParameterSources()
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
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var content = new StringContent("{}", new UTF8Encoding(false), "application/json");

            var response = await client.PostAsync("/apicontroller", content);

            Assert.Equal(StatusCodes.Status201Created, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use API-Controller to infer content-tpye multipart/form-data")]
        public async Task UseApiControllerToInferContentTypeMultipartFormData()
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
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var content = new MultipartFormDataContent();
            var payload = Encoding.UTF8.GetBytes("Test");
            content.Add(
                new StreamContent(File.OpenRead(@"TestData\example.txt")),
                "file", // Must match parameter name
                "example.txt");

            var response = await client.PostAsync("/apicontroller/form-postback", content);

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }
    }
}
