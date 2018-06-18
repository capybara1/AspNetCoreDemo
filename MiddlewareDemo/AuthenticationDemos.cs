using AspNetCoreDemo.MiddlewareDemo.Security.Authentication;
using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.MiddlewareDemo
{
    [Trait("Category", "ASP.NET Core Middleware / Authentication")]
    public class AuthenticationDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AuthenticationDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use authentication without default scheme")]
        public async Task UseAuthenticationWithoutDefaultScheme()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddTransient<DemoAuthenticationHandler>();
                    services.AddAuthentication()
                        .AddScheme<DemoAuthenticationSchemeOptions, DemoAuthenticationHandler>("DemoScheme", opt => { });
                })
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        var result = await context.AuthenticateAsync("DemoScheme");
                        if (result.None)
                        {
                            await context.ChallengeAsync("DemoScheme");
                        }
                        else if (result.Succeeded)
                        {
                            context.User = result.Principal;
                            await next();
                        }
                        else
                        {
                            await context.ForbidAsync("DemoScheme");
                        }
                    });
                    app.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("Hello, World!", new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var unauthorizedResponse = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status401Unauthorized, (int)unauthorizedResponse.StatusCode);

            var authenticateHeaderValue = unauthorizedResponse.Headers.GetValues("WWW-Authenticate").First();
            Assert.Equal("DemoScheme", authenticateHeaderValue);

            client.DefaultRequestHeaders.Add("Proof", "False Proof");
            var forbiddenResponse = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status403Forbidden, (int)forbiddenResponse.StatusCode);

            client.DefaultRequestHeaders.Remove("Proof");
            client.DefaultRequestHeaders.Add("Proof", "Sound Proof");
            var acceptedResponse = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status200OK, (int)acceptedResponse.StatusCode);
        }

        [Fact(DisplayName = "Use authentication with default scheme")]
        public async Task UseAuthenticationWithDefaultScheme()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddTransient<DemoAuthenticationHandler>();
                    services.AddAuthentication("DemoScheme") // Defines which scheme is the default
                        .AddScheme<DemoAuthenticationSchemeOptions, DemoAuthenticationHandler>("DemoScheme", opt => { });
                })
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        var result = await context.AuthenticateAsync();
                        if (result.None)
                        {
                            await context.ChallengeAsync();
                        }
                        else if (result.Succeeded)
                        {
                            context.User = result.Principal;
                            await next();
                        }
                        else
                        {
                            await context.ForbidAsync();
                        }
                    });
                    app.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("Hello, World!", new UTF8Encoding(false));
                    });
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var unauthorizedResponse = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status401Unauthorized, (int)unauthorizedResponse.StatusCode);

            var authenticateHeaderValue = unauthorizedResponse.Headers.GetValues("WWW-Authenticate").First();
            Assert.Equal("DemoScheme", authenticateHeaderValue);

            client.DefaultRequestHeaders.Add("Proof", "False Proof");
            var forbiddenResponse = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status403Forbidden, (int)forbiddenResponse.StatusCode);

            client.DefaultRequestHeaders.Remove("Proof");
            client.DefaultRequestHeaders.Add("Proof", "Sound Proof");
            var acceptedResponse = await client.GetAsync("/");
            Assert.Equal(StatusCodes.Status200OK, (int)acceptedResponse.StatusCode);
        }

        [Fact(DisplayName = "Use authentication with sign-in")]
        public async Task UseAuthenticationWithoutSignIn()
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                    services.AddTransient<DemoAuthenticationHandler>();
                    services.AddAuthentication()
                        .AddScheme<DemoAuthenticationSchemeOptions, DemoAuthenticationHandler>("DemoScheme", opt => { })
                        .AddCookie("Cookie");
                })
                .Configure(app =>
                {
                    var serviceProvider = app.ApplicationServices;

                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        var result = await context.AuthenticateAsync("DemoScheme");
                        if (result.None)
                        {
                            await context.ChallengeAsync("DemoScheme");
                        }
                        else if (result.Succeeded)
                        {
                            context.User = result.Principal;
                            await next();
                        }
                        else
                        {
                            await context.ForbidAsync("DemoScheme");
                        }
                    });
                    app.Map("/sign_in", mappedApp =>
                    {
                        mappedApp.Run(async context =>
                        {
                            await context.SignInAsync(
                                "Cookie",
                                context.User);

                            context.Response.StatusCode = StatusCodes.Status204NoContent;
                        });
                    });
                    app.Map("/sign_out", mappedApp =>
                    {
                        mappedApp.Run(async context =>
                        {
                            await context.SignOutAsync("Cookie");

                            context.Response.StatusCode = StatusCodes.Status204NoContent;
                        });
                    });
                    app.Map("/authn", mappedApp =>
                    {
                        mappedApp.Run(async context =>
                        {
                            var localLogger = serviceProvider.GetRequiredService<ILogger<AuthorizationDemos>>();

                            var result = await context.AuthenticateAsync("Cookie");
                            var principal = result.Principal;
                            var primaryIdentity = principal.Identity;
                            localLogger.LogInformation($"Is authenticated: {primaryIdentity.IsAuthenticated}");
                            if (primaryIdentity is ClaimsIdentity claimsIdentity)
                            {
                                foreach (var claim in claimsIdentity.Claims)
                                {
                                    localLogger.LogInformation($"Value of claim of type '{claim.Type}' issued by {claim.Issuer}: {claim.Value}");
                                }
                            }

                            context.Response.StatusCode = StatusCodes.Status204NoContent;
                        });
                    });
                });

            var server = new TestServer(builder);

            var logger = server.Host.Services.GetRequiredService<ILogger<BasicDemos>>();
            
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Proof", "Sound Proof");

            var signInResponse = await client.GetAsync("/sign_in");
            Assert.Equal(StatusCodes.Status204NoContent, (int)signInResponse.StatusCode);

            var setCookieHeaderValue = signInResponse.Headers.GetValues("Set-Cookie").First();
            logger.LogInformation($"Set-Cookie Header: {setCookieHeaderValue}");
            client.DefaultRequestHeaders.Add("Cookie", setCookieHeaderValue);

            var authenticationWithCookieResponse = await client.GetAsync("/authn");
            Assert.Equal(StatusCodes.Status204NoContent, (int)authenticationWithCookieResponse.StatusCode);

            var signOutResponse = await client.GetAsync("/sign_out");
            Assert.Equal(StatusCodes.Status204NoContent, (int)signOutResponse.StatusCode);

            var removeCookieHeaderValue = signOutResponse.Headers.GetValues("Set-Cookie").First();
            logger.LogInformation($"Set-Cookie Header: {removeCookieHeaderValue}");
            
            var authenticationWithoutCookieResponse = await client.GetAsync("/authn");
            Assert.Equal(StatusCodes.Status204NoContent, (int)authenticationWithoutCookieResponse.StatusCode);

        }
    }
}
