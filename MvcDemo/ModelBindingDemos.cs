using AspNetCoreDemo.MvcDemo.Binder;
using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.MvcDemo
{
    [Trait("Category", "ASP.NET Core MVC / Model Binding")]
    public class ModelBindingDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ModelBindingDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use Controller with Parameter Class")]
        public async Task UseControllerWithParameterClass()
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

            var content = new StringContent(
                "{ Text: 'my text' }",
                Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync("be/test1/John", content);

            Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use Controller with Inline Parameters")]
        public async Task UseControllerWithInlineParameters()
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

            var content = new StringContent(
                "{ Text: 'my text' }",
                Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync("be/test2/John", content);

            Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use Controller with Failed Model Validation")]
        public async Task UseControllerWithFailedModelValidation()
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
                        .AddJsonFormatters()
                        .AddDataAnnotations(); // Required
                })
                .Configure(app =>
                {
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var content = new StringContent(
                "{ Priority: 5, Text: 'My text' }",
                Encoding.UTF8,
                "application/json");
            content.Headers.ContentType.MediaType = "application/json";
            var response = await client.PutAsync("be/test1/a_very_long_name", content);

            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use Controller with array parameter")]
        public async Task UseControllerWithArrayParameter()
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
            
            var response = await client.GetAsync("be/array_parameter?param=value1&param=value2");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use Controller with complex parameter")]
        public async Task UseControllerWithComplexParameter()
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
            
            var response = await client.GetAsync("be/complex_parameter?example.text=example%20text&example.priority=2");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }
        
        [Fact(DisplayName = "Use Controller with custom binder")]
        public async Task UseControllerWithCustomBinder()
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
                            setup.ModelBinderProviders.Insert(0, new CustomModelBinderProvider());
                        })
                        .AddJsonFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var content = new StringContent(
                "{ Priority: 5, Text: 'My text' }",
                Encoding.UTF8,
                "application/json");
            content.Headers.ContentType.MediaType = "application/json";
            var response = await client.PutAsync("be/custom_binder", content);

            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        }
    }
}
