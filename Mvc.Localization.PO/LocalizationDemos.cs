using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Mvc.Localization.PO
{
    [Trait("Category", "ASP.NET Core MVC / Localization")]
    public class LocalizationDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public LocalizationDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use string localization with PO file")]
        public async Task UseStringLocalizationWithResxFile()
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
                        .AddMvcLocalization();

                    services.AddPortableObjectLocalization();
                })
                .Configure(app =>
                {
                    app.UseRequestLocalization(opt =>
                    {
                        var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("de"),
                            new CultureInfo("en"),
                        };

                        opt.DefaultRequestCulture = new RequestCulture("en");
                        opt.SupportedCultures = supportedCultures;
                        opt.SupportedUICultures = supportedCultures;
                    });
                    app.UseMvc();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var headerValue = new StringWithQualityHeaderValue("de");
            client.DefaultRequestHeaders.AcceptLanguage.Add(headerValue);

            await client.GetAsync("localization");
        }
    }
}
