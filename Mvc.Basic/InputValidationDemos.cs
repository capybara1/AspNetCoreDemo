using AspNetCoreDemo.Mvc.Basic.Binder;
using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Mvc.Basic
{
    [Trait("Category", "ASP.NET Core MVC / Input Validation")]
    public class InputValidationDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public InputValidationDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }
        
        [Fact(DisplayName = "Perform model validation within custom binding")]
        public async Task PerformModelValidationWithinCustomBinding()
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
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                        .AddJsonFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            
            var response = await client.GetAsync("validation/custom_binder?filter=#invalid");

            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use model metadata provider to modify binding behavior")]
        public async Task UseModelMetadataProviderToModifyBindingBehavior()
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
                            setup.ModelMetadataDetailsProviders.Add(new MetadataProviders.RequireQueryParameters());
                        })
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                        .AddJsonFormatters();
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("validation/required_query");

            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var logger = server.Host.Services.GetRequiredService<ILogger<InputValidationDemos>>();
            logger.LogInformation(content);
        }
    }
}
