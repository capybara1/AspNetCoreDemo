using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
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

        [Fact(DisplayName = "Use Accept-Language header to infer request culture")]
        public async Task UseAccceptLanguageHeaderToInferRequestCulture()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    // No service configuration required
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

                    app.Run(async context =>
                    {
                        var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                        var requestCulture = requestCultureFeature.RequestCulture;
                        var culture = requestCulture.Culture;

                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(culture.TwoLetterISOLanguageName, new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var headerValue = new StringWithQualityHeaderValue("de");
            client.DefaultRequestHeaders.AcceptLanguage.Add(headerValue);

            var response = await client.GetAsync("/");
            var readCulture = await response.Content.ReadAsStringAsync();

            Assert.Equal("de", readCulture);
        }

        [Fact(DisplayName = "Use cookie to infer request culture")]
        public async Task UseCookieToInferRequestCulture()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    // No service configuration required
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

                    app.Run(async context =>
                    {
                        var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                        var requestCulture = requestCultureFeature.RequestCulture;
                        var culture = requestCulture.Culture;

                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(culture.TwoLetterISOLanguageName, new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var cookieValue = CreateCookie("de");

            client.DefaultRequestHeaders.Add("Cookie", cookieValue);

            var response = await client.GetAsync("/");
            var readCulture = await response.Content.ReadAsStringAsync();

            Assert.Equal("de", readCulture);
        }

        [Fact(DisplayName = "Use query parameter to infer request culture")]
        public async Task UseQueryParameterToInferRequestCulture()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    // No service configuration required
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

                    app.Run(async context =>
                    {
                        var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                        var requestCulture = requestCultureFeature.RequestCulture;
                        var culture = requestCulture.Culture;

                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(culture.TwoLetterISOLanguageName, new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();
            
            var response = await client.GetAsync("/?culture=de");
            var readCulture = await response.Content.ReadAsStringAsync();

            Assert.Equal("de", readCulture);
        }

        private static string CreateCookie(string culture)
        {
            var requestCulture = new RequestCulture(culture);
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);
            var result = $"{CookieRequestCultureProvider.DefaultCookieName}={cookieValue}";

            return result;
        }
    }
}
