using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Mvc.Cors
{
    [Trait("Category", "ASP.NET Core MVC / CORS")]
    public class CorsDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CorsDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use CORS policy by attributation")]
        public async Task UsingCorsPolicyByAttributation()
        {
            var builder = new WebHostBuilder()
            .ConfigureLogging(setup =>
            {
                setup.AddDebug();
                setup.SetupDemoLogging(_testOutputHelper);
                setup.SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureServices(services =>
            {
                services.AddLogging();
                services.AddCors(opt => opt.AddPolicy( // Requires Microsoft.AspNetCore.Cors package, fetched as dependency of the Microsoft.AspNetCore.Mvc.Cors package
                    "DemoCorsPolicy",
                    policyBuilder => policyBuilder
                        .WithMethods("OPTIONS", "GET", "PUT")
                        .WithOrigins("http://example.com")
                        .AllowAnyHeader()));
                services.AddMvcCore()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddCors(); // Attention: This is not the middleware! Requires the Microsoft.AspNetCore.Mvc.Cors package
            })
            .Configure(app =>
            {
                app.UseMvc();
            });
            
            var server = new TestServer(builder);

            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            client.DefaultRequestHeaders.Add("Origin", "http://example.com");
            client.DefaultRequestHeaders.Add("Referer", "http://example.com");
            
            var response = await client.GetAsync("/enabled");
            AssertOriginIsAllowed(response, "http://example.com");

            var otherResponse = await client.GetAsync("/disabled");
            AssertOriginIsDisallowed(otherResponse, "http://example.com");
        }

        private static void AssertOriginIsAllowed(HttpResponseMessage response, string origin)
        {
            var headerExists = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var allowOriginHeaderValues);
            Assert.True(headerExists);
            Assert.Contains(origin, allowOriginHeaderValues?.FirstOrDefault());
        }

        private static void AssertOriginIsDisallowed(HttpResponseMessage response, string origin)
        {
            var headerExists = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var allowOriginHeaderValues);
            Assert.False(headerExists);
        }
    }
}
