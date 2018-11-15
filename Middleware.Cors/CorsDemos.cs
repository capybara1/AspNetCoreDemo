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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Middleware.Cors
{
    // CORS is a security protocol,
    // usuall understood by web-browsers,
    // that allows design of an access-control policy
    // which allows cross-origin-requests,
    // that would otherwise default to the strict same-origin policy.
    // Cross-origin-requests might be required for some user experiences
    // but opens up attack vectors such as e.g. Cross-Site Request Forgery (CSRF).
    // Thus controlling access, in combination with additional measured such as
    // e.g. a nonce (CSFR-Token), is necessary to keep risk at bay.

    // CSRF attacks are ranked among the top 10 of most critical attacks by OWASP in 2013.
    // In 2017 the top 10 didn't include CSRF anymore, because many frameworks
    // included CSRF defenses.
    // Note that proper protection is usually achieved by multiple measures.
    // See https://www.owasp.org/images/7/72/OWASP_Top_10-2017_%28en%29.pdf.pdf.

    // Resources for same-origin policy
    // * https://www.w3.org/Security/wiki/Same_Origin_Policy
    // * https://developer.mozilla.org/en-US/docs/Web/Security/Same-origin_policy

    // Resources for CSRF
    // * https://en.wikipedia.org/wiki/Cross-site_request_forgery
    // * https://www.owasp.org/index.php/Cross-Site_Request_Forgery_(CSRF)
    // * https://www.youtube.com/watch?v=vRBihr41JTo
    // * https://www.youtube.com/watch?v=MBOMBZS2u-I

    // Resources for CORS
    // * https://www.w3.org/2001/tag/2010/06/01-cross-domain.html
    // * https://code.google.com/archive/p/browsersec/wikis/Part2.wiki
    // * https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.1#how-cors-works

    [Trait("Category", "ASP.NET Core Middleware / CORS")]
    public class CorsDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CorsDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Accepted Request")]
        public async Task AcceptedRequest()
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
                services.AddCors(); // Requires Microsoft.AspNetCore.Cors package
            })
            .Configure(app =>
            {
                var serviceProvider = app.ApplicationServices;

                app.UseCors(policyBuilder => policyBuilder
                    .WithMethods("OPTIONS", "GET", "PUT", "POST")
                    .WithOrigins("http://example.com")
                    .AllowAnyHeader());
                app.Run(async context =>
                {
                    var localLogger = serviceProvider.GetRequiredService<ILogger<CorsDemos>>();

                    switch (context.Request.Method)
                    {
                        case "OPTIONS":
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            break;

                        case "GET":
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync("Hello, World!", new UTF8Encoding(false));
                            break;

                        case "PUT":
                            context.Response.StatusCode = StatusCodes.Status204NoContent;
                            break;

                        case "POST":
                            context.Response.Headers.Add("Location", "http://localhost/1");
                            context.Response.StatusCode = StatusCodes.Status201Created;
                            break;

                        default:
                            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            break;

                    }
                });
            });

            var server = new TestServer(builder);

            var logger = server.Host.Services.GetRequiredService<ILogger<CorsDemos>>();

            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Origin", "http://example.com");

            var simpleGetRequest = PrepareSimpleGetRequest();
            var simpleGetResponse = await client.SendAsync(simpleGetRequest);
            AssertOriginIsAllowed(simpleGetResponse, "http://example.com");

            var simplePostContent = PrepareSimplePostContent();
            var simplePostResponse = await client.PostAsync("/", simplePostContent);
            AssertOriginIsAllowed(simplePostResponse, "http://example.com");

            var preflightRequest = PreparePreflightRequest("PUT", "Content-Type");
            var preflightResponse = await client.SendAsync(preflightRequest);
            AssertOriginIsAllowed(preflightResponse, "http://example.com");
            AssertMethodIsAllowed(preflightResponse, "PUT");
            AssertHeaderIsAllowed(preflightResponse, "Content-Type");
        }

        private static HttpRequestMessage PrepareSimpleGetRequest()
        {
            // Most relevant GET request headers considered simple:
            // * Accept
            // * Accept-Language

            // See also https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS

            var request = new HttpRequestMessage(HttpMethod.Get, "/");

            var acceptedMediaType = MediaTypeWithQualityHeaderValue.Parse("text/plain");
            request.Headers.Accept.Add(acceptedMediaType);

            var acceptedLanguage = StringWithQualityHeaderValue.Parse("en-us");
            request.Headers.AcceptLanguage.Add(acceptedLanguage);

            return request;
        }

        private static StringContent PrepareSimplePostContent()
        {
            // Most relevant POST request headers considered simple:
            // * Content-Type
            // * Content-Language

            // Content-Type header values considered simple:
            // * text/plain
            // * application/x-www-form-urlencoded
            // * multipart/form-data

            // See also https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS

            var content = new StringContent("", new UTF8Encoding(false), "text/plain");

            content.Headers.ContentLanguage.Add("en-us");

            return content;
        }

        private static HttpRequestMessage PreparePreflightRequest(string method, params string[] headers)
        {
            var request = new HttpRequestMessage(HttpMethod.Options, "/");

            request.Headers.Add("Access-Control-Request-Method", method);
            request.Headers.Add("Access-Control-Request-Headers", headers);

            return request;
        }

        private static void AssertOriginIsAllowed(HttpResponseMessage response, string origin)
        {
            var headerExists = response.Headers.TryGetValues("Access-Control-Allow-Origin", out var allowOriginHeaderValues);
            Assert.True(headerExists);
            Assert.Contains(origin, allowOriginHeaderValues?.FirstOrDefault());
        }

        private static void AssertMethodIsAllowed(HttpResponseMessage response, string method)
        {
            var headerExists = response.Headers.TryGetValues("Access-Control-Allow-Methods", out var allowMethodsHeaderValues);
            Assert.True(headerExists);
            Assert.Contains(method, allowMethodsHeaderValues);
        }

        private static void AssertHeaderIsAllowed(HttpResponseMessage response, string header)
        {
            var headerExists = response.Headers.TryGetValues("Access-Control-Allow-Headers", out var allowHeadersHeaderValues);
            Assert.True(headerExists);
            Assert.Contains(header, allowHeadersHeaderValues?.FirstOrDefault());
        }
    }
}
