using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Middleware.DataProtection
{
    [Trait("Category", "ASP.NET Core Middleware / Data Protection")]
    public class DataProtectionDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DataProtectionDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Data Protection")]
        public async Task DataProtection()
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
                services.AddDataProtection(); // Requires Microsoft.AspNetCore.DataProtection package
            })
            .Configure(app =>
            {
                var serviceProvider = app.ApplicationServices;
                
                app.Run(async context =>
                {
                    var localLogger = serviceProvider.GetRequiredService<ILogger<DataProtectionDemos>>();
                    var provider = serviceProvider.GetRequiredService<IDataProtectionProvider>();
                    var protector = provider.CreateProtector("Local Purpose String");

                    switch (context.Request.Method)
                    {
                        case "GET":
                            var protectedData = protector.Protect("Hello, World!");

                            context.Response.StatusCode = StatusCodes.Status200OK;
                            context.Response.ContentType = "text/plain";
                            await context.Response.WriteAsync(protectedData, new UTF8Encoding(false));
                            break;

                        case "PUT":
                            string bodyContent;
                            using (var reader = new StreamReader(context.Request.Body, new UTF8Encoding(false), true, 1024, true))
                            {
                                bodyContent = await reader.ReadToEndAsync();
                            }

                            var unprotectContent = protector.Unprotect(bodyContent);

                            localLogger.LogInformation($"Unprotected: {unprotectContent}");

                            context.Response.StatusCode = StatusCodes.Status204NoContent;
                            break;

                        default:
                            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            break;

                    }
                });
            });

            var server = new TestServer(builder);

            var logger = server.Host.Services.GetRequiredService<ILogger<DataProtectionDemos>>();

            var client = server.CreateClient();

            var headerValue = System.Net.Http.Headers.StringWithQualityHeaderValue.Parse("gzip");
            client.DefaultRequestHeaders.AcceptEncoding.Add(headerValue);

            var protectResponse = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status200OK, (int)protectResponse.StatusCode);

            var data = await protectResponse.Content.ReadAsStringAsync();

            logger.LogInformation($"Protected: {data}");

            var content = new StringContent(data, new UTF8Encoding(false), "text/plain");
            var unprotectResponse = await client.PutAsync("/", content);
            Assert.Equal(StatusCodes.Status204NoContent, (int)unprotectResponse.StatusCode);
        }
    }
}
