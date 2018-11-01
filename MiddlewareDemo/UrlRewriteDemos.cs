using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.MiddlewareDemo
{
    [Trait("Category", "ASP.NET Core Middleware / URL Rewrite")]
    public class UrlRewriteDemos
    {
        // Note recommendations:
        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/url-rewriting?view=aspnetcore-2.1#when-to-use-url-rewriting-middleware

        private readonly ITestOutputHelper _testOutputHelper;

        public UrlRewriteDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use rewrite rule")]
        public async Task UseRewriteRule()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .Configure(app =>
                {
                    var options = new RewriteOptions()
                        .AddRewrite(
                            @"^rewritable/(\w+)",
                            "rewritten?param=$1",
                            skipRemainingRules: true);

                    app.UseRewriter(options);

                    app.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync(
                            context.Request.Query["param"].First(),
                            new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("rewritable/test");
            Assert.Equal(200, (int)response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("test", content);
        }

        [Fact(DisplayName = "Use redirect rule")]
        public async Task UseRedirectRule()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .Configure(app =>
                {
                    var options = new RewriteOptions()
                        .AddRedirect("redirectable/(.*)", "redirected/$1");

                    app.UseRewriter(options);
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("redirectable/test");
            Assert.Equal(302, (int)response.StatusCode);
            Assert.Equal("/redirected/test", response.Headers.Location.ToString());
        }
    }
}