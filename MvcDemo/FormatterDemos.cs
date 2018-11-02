using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.MvcDemo
{
    [Trait("Category", "ASP.NET Core MVC / Formatters")]
    public class FormatterDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FormatterDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use Controller with Json Input Formatting")]
        public async Task UseControllerWithJsonInputFormatting()
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
                        .AddJsonFormatters()
                        .AddXmlSerializerFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var content = new StringContent(
                "{ Text: \"my text\" }",
                Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync("formatter", content);

            Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use Controller with Xml Input Formatting")]
        public async Task UseControllerWithXmlInputFormatting()
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
                        .AddJsonFormatters()
                        .AddXmlSerializerFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var content = new StringContent(
                "<ExampleModel><Text>my text</Text></ExampleModel>",
                Encoding.UTF8,
                "application/xml");
            var response = await client.PutAsync("formatter", content);

            Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use Controller with Json Output Formatting")]
        public async Task UseControllerWithJsonOutputFormatting()
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
                        .AddJsonFormatters()
                        .AddXmlSerializerFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            var acceptHeaderValue = MediaTypeWithQualityHeaderValue.Parse("application/json");
            client.DefaultRequestHeaders.Accept.Add(acceptHeaderValue);
            var response = await client.GetAsync("formatter");
            var example = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("{\"priority\":2,\"text\":\"example text\"}", example);
        }

        [Fact(DisplayName = "Use Controller with Xml Output Formatting")]
        public async Task UseControllerWithXmlOutputFormatting()
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
                        .AddJsonFormatters()
                        .AddXmlSerializerFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);
            
            var client = server.CreateClient();
            var acceptHeaderValue = MediaTypeWithQualityHeaderValue.Parse("application/xml");
            client.DefaultRequestHeaders.Accept.Add(acceptHeaderValue);
            var response = await client.GetAsync("formatter");
            var example = await response.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            Assert.Equal("<ExampleModel xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Priority>2</Priority><Text>example text</Text></ExampleModel>", example);
        }
    }
}
