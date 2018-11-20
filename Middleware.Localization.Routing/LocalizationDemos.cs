using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Middleware.Localization
{
    [Trait("Category", "ASP.NET Core MVC / Localization")]
    public class LocalizationDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public LocalizationDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use route value to infer request culture")]
        public async Task UseRouteValueToInferRequestCulture()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                })
                .Configure(app =>
                {
                    app.UseRouter(routes =>
                    {
                        // For reserved routing names see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-2.1#reserved-routing-names
                        routes.MapMiddlewareRoute("test/{culture}", fork =>
                        {
                            fork.UseRequestLocalization(opt =>
                            {
                                var supportedCultures = new List<CultureInfo>
                                {
                                    new CultureInfo("de"),
                                    new CultureInfo("en"),
                                };

                                opt.DefaultRequestCulture = new RequestCulture("en");
                                opt.SupportedCultures = supportedCultures;
                                opt.SupportedUICultures = supportedCultures;

                                opt.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider());
                            });

                            fork.Run(async context =>
                            {
                                var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                                var requestCulture = requestCultureFeature.RequestCulture;
                                var culture = requestCulture.Culture;

                                context.Response.StatusCode = StatusCodes.Status200OK;
                                context.Response.ContentType = "text/plain";
                                await context.Response.WriteAsync(culture.TwoLetterISOLanguageName, new UTF8Encoding(false));
                            });
                        });
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            
            var response = await client.GetAsync("/test/de");
            var readCulture = await response.Content.ReadAsStringAsync();

            Assert.Equal("de", readCulture);
        }
    }
}
