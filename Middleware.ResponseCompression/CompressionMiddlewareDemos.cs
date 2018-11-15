using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Middleware.ResponseCompression
{
    [Trait("Category", "ASP.NET Core Middleware / Compression Middleware")]
    public class CompressionMiddlewareDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CompressionMiddlewareDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Response Compression")]
        public async Task ResponseCompression()
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
                services
                    .AddLogging()
                    .AddResponseCompression(); // Requires Microsoft.AspNetCore.ResponseCompression package
            })
            .Configure(app =>
            {
                app
                    .UseResponseCompression()
                    .Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("Hello, World!", new UTF8Encoding(false));
                    });
            });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var headerValue = System.Net.Http.Headers.StringWithQualityHeaderValue.Parse("gzip");
            client.DefaultRequestHeaders.AcceptEncoding.Add(headerValue);

            var response = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);

            var contentEncoding = response.Content.Headers.ContentEncoding.FirstOrDefault();
            Assert.Equal("gzip", contentEncoding);

            string text;
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress, true))
            using (var reader = new StreamReader(gzipStream, new UTF8Encoding(false), false, 1024, true))
            {
                text = await reader.ReadToEndAsync();
            }
            Assert.Equal("Hello, World!", text);
        }
    }
}
