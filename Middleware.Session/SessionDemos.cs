using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Middleware.Session
{
    [Trait("Category", "ASP.NET Core Middleware / Session")]
    public class SessionDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SessionDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use session store")]
        public async Task UseSessionStore()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    // In production the distributed cache must be
                    // a real solution, shared by the nodes of the
                    // cluster
                    services.AddDistributedMemoryCache();

                    services.AddSession(options =>
                    {
                        options.IdleTimeout = TimeSpan.FromSeconds(3);
                        options.Cookie.HttpOnly = true;
                    });
                })
                .Configure(app =>
                {
                    app
                        .UseSession()
                        .Run(async context =>
                        {
                            var counter = context.Session.GetInt32("counter");
                            if (counter == null)
                            {
                                counter = 0;
                            }
                            counter += 1;
                            context.Session.SetInt32("counter", counter.Value);

                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync($"Current count is {counter}", new UTF8Encoding(false));
                        });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var firstResponse = await client.GetAsync("/");
            var content = await firstResponse.Content.ReadAsStringAsync();
            var cookie = firstResponse.Headers.GetValues("Set-Cookie").First();

            Assert.Equal(StatusCodes.Status200OK, (int)firstResponse.StatusCode);
            Assert.Equal("Current count is 1", content);

            var secondRequest = new HttpRequestMessage(HttpMethod.Get, "/");
            secondRequest.Headers.Add("Cookie", cookie);
            var secondResponse = await client.SendAsync(secondRequest);
            content = await secondResponse.Content.ReadAsStringAsync();

            Assert.Equal(StatusCodes.Status200OK, (int)secondResponse.StatusCode);
            Assert.Equal("Current count is 2", content);
        }
    }
}
