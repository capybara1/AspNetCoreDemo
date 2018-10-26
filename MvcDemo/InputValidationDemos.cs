using AspNetCoreDemo.MvcDemo.Binder;
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
    [Trait("Category", "ASP.NET Core MVC / Input Validation")]
    public class InputValidationDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public InputValidationDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }
        
        [Fact(DisplayName = "Use data annotations for model validation")]
        public async Task UseDataAnnotationsForModelValidation()
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
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            
            var response = await client.GetAsync("validation/annotated_model?value=a_very_long_name");

            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        }
        
        [Fact(DisplayName = "Use custom binder for model validation")]
        public async Task UseCustomBinderForModelValidation()
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
            
            var response = await client.GetAsync("validation/custom_binder?filter=#invalid");

            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        }
    }
}
