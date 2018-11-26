using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Mvc.Basic
{
    [Trait("Category", "ASP.NET Core Middleware / Antiforgery")]
    public class AntiforgeryDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AntiforgeryDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Protect from CSRF using Form data")]
        public async Task ProtectFromCsrfUsingFormData()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddDataProtection();
                    services.AddAntiforgery();
                })
                .Configure(app =>
                {
                    var services = app.ApplicationServices;
                    var antiforgery = services.GetRequiredService<IAntiforgery>();

                    app.Run(async context =>
                    {
                        if (context.Request.Method == "GET")
                        {
                            // Sets the cookie containing the Cookie-Token.
                            // The return value exposes the Request-Token:
                            var tokens = antiforgery.GetAndStoreTokens(context);

                            // The Request-Token is expected to be embedded
                            // as hidden input in the form contained in the HTML response.
                            // In order to keep the example simple, no HTML is used in
                            // this demonstration:
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync(tokens.RequestToken);
                        }
                        else
                        {
                            await antiforgery.ValidateRequestAsync(context);

                            context.Response.StatusCode = StatusCodes.Status204NoContent;
                        }
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var getResponse = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status200OK, (int)getResponse.StatusCode);

            var tokenCookie = getResponse.Headers.GetValues("Set-Cookie")
                .First(cookie => cookie.StartsWith(".AspNetCore.Antiforgery."));

            var requestToken = await getResponse.Content.ReadAsStringAsync();
            
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/");
            postRequest.Headers.Add("Cookie", tokenCookie);
            var data = new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = requestToken,
            };
            postRequest.Content = new FormUrlEncodedContent(data);
            var postResponse = await client.SendAsync(postRequest);
            Assert.Equal(StatusCodes.Status204NoContent, (int)postResponse.StatusCode);
        }

        [Fact(DisplayName = "Protect from CSRF using Angular convention")]
        public async Task ProtectFromCsrfUsingAngularConvention()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddDataProtection();
                    services.AddAntiforgery(opt =>
                    {
                        opt.HeaderName = "X-XSRF-TOKEN";
                    });
                })
                .Configure(app =>
                {
                    var services = app.ApplicationServices;
                    var antiforgery = services.GetRequiredService<IAntiforgery>();

                    app.Run(async context =>
                    {
                        if (context.Request.Method == "GET")
                        {
                            // Sets the cookie containing the Cookie-Token.
                            // The return value exposes the Request-Token:
                            var tokens = antiforgery.GetAndStoreTokens(context);

                            // The Request-Token is expected to be communicated via
                            // a cookie, identified by the name XSRF-TOKEN,
                            // in this scenario:
                            context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                                new CookieOptions() { HttpOnly = false });
                        }
                        else
                        {
                            await antiforgery.ValidateRequestAsync(context);
                        }

                        context.Response.StatusCode = StatusCodes.Status204NoContent;
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var getResponse = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status204NoContent, (int)getResponse.StatusCode);

            var tokenCookie = getResponse.Headers.GetValues("Set-Cookie")
                .First(cookie => cookie.StartsWith(".AspNetCore.Antiforgery."));

            var requestToken = getResponse.Headers.GetValues("Set-Cookie")
                .Where(cookie => cookie.StartsWith("XSRF-TOKEN"))
                .Select(GetCookieValue)
                .First();

            // The Request-Token is expected to be communicated via
            // the X-XSRF-TOKEN request header in this scenario:
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/");
            postRequest.Headers.Add("Cookie", tokenCookie);
            postRequest.Headers.Add("X-XSRF-TOKEN", requestToken);
            var postResponse = await client.SendAsync(postRequest);

            Assert.Equal(StatusCodes.Status204NoContent, (int)postResponse.StatusCode);
        }

        private static string GetCookieValue(string cookie)
        {
            return cookie.Substring(11).Split(';')[0];
        }
    }
}
