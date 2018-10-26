using AspNetCoreDemo.MvcDemo.Binder;
using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.MvcDemo
{
    [Trait("Category", "ASP.NET Core MVC / Parameter Model Binding")]
    public class ParameterModelBindingDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ParameterModelBindingDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use Controller with plain, simple parameters")]
        public async Task UseControllerWithPlainSimpleParameters()
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
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            // Form values have precedence over route values
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("priority", "1"),
            });

            // Route values have precedence over query parameters
            var response = await client.PostAsync("pb/simple_parameter/plain/99?text=example%20text&priority=2", content);

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use Controller with annotated, simple parameters")]
        public async Task UseControllerWithSimpleParameters()
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
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("priority", "1"),
            });
            
            var response = await client.PostAsync("pb/simple_parameter/annotated/99?text=example%20text&priority=2", content);

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
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
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("pb/array_parameter?param=value1&param=value2");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use Controller with binding to complex object by qualified parameters")]
        public async Task UseControllerWithBindingToComplexObjectByQualifiedParameter()
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
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("priority", "1"),
            });
            
            var response = await client.PostAsync("pb/complex_parameter/qualified/99?example.text=example%20text&example.priority=2", content);

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use Controller with binding to complex object by unqualified parameters")]
        public async Task UseControllerWithBindingToComplexObjectByUnqualifiedParameter()
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
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("priority", "1"),
            });

            var response = await client.PostAsync("pb/complex_parameter/unqualified/99?text=example%20text&priority=2", content);

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
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            
            var response = await client.GetAsync("pb/custom_binder?filter=-test");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }
    }
}
