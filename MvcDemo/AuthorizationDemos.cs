using AspNetCoreDemo.MvcDemo.Security.Authentication;
using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.MvcDemo
{
    [Trait("Category", "ASP.NET Core MVC / Authorization")]
    public class AuthorizationDemos
    {
        private class TestParameters
        {
            public string Route { get; set; }
            public bool Authenticate { get; set; } = true;
            public int ExpectedStatusCode { get; set; } = StatusCodes.Status204NoContent;
        } 

        private readonly ITestOutputHelper _testOutputHelper;

        public AuthorizationDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Use action that allows anonymous access")]
        public async Task UseActionThatAllowsAnonymousAccess()
        {
            await RunTestAsync(new TestParameters
            {
                Route = "/authz/anonymous",
                Authenticate = false,
            });
        }

        [Fact(DisplayName = "Use protected action without authentication")]
        public async Task UseProtectedActionWithoutAuthentication()
        {
            await RunTestAsync(new TestParameters
            {
                Route = "/authz",
                Authenticate = false,
                ExpectedStatusCode = StatusCodes.Status401Unauthorized,
            });
        }

        [Fact(DisplayName = "Use declarative authorization for default scheme")]
        public async Task UseDeclarativeAuthorizationForDefaultScheme()
        {
            await RunTestAsync(new TestParameters
            {
                Route = "/authz",
            });
        }

        [Fact(DisplayName = "Use declarative authorization for specific scheme")]
        public async Task UseDeclarativeAuthorizationForSpecificScheme()
        {
            await RunTestAsync(new TestParameters
            {
                Route = "/authz/specific",
            });
        }

        [Fact(DisplayName = "Use declarative authorization with set of role alternatives")]
        public async Task UseAuthorizationWithSetOfRoleAlternatives()
        {
            await RunTestAsync(new TestParameters
            {
                Route = "/authz/roles/any",
            });
        }

        [Fact(DisplayName = "Use declarative authorization with set of required roles")]
        public async Task UseAuthorizationWithSetOfRequiredRoles()
        {
            await RunTestAsync(new TestParameters
            {
                Route = "/authz/roles/all",
                ExpectedStatusCode = StatusCodes.Status403Forbidden,
            });
        }

        [Fact(DisplayName = "Use declarative authorization with a policy")]
        public async Task UseDeclarativeAuthorizationWithPolicies()
        {
            await RunTestAsync(new TestParameters
            {
                Route = "/authz/policy",
            });
        }

        private async Task RunTestAsync(TestParameters parameters)
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddAuthentication("DemoScheme")
                        .AddScheme<DemoAuthenticationSchemeOptions, DemoAuthenticationHandler>("DemoScheme", opt => { });
                    services.AddMvcCore()
                        .AddAuthorization(opt => opt.AddPolicy( // Attention: This is not the middleware!
                            "DemoPolicy",
                            policyBuilder => policyBuilder
                                .RequireRole("Contributor")));
                })
                .Configure(app =>
                {
                    app.UseAuthentication();
                    app.UseMvcWithDefaultRoute();
                });

            var server = new TestServer(builder);

            var client = server.CreateClient();

            if (parameters.Authenticate)
            {
                client.DefaultRequestHeaders.Add("Proof", "Sound Proof");
            }

            var response = await client.GetAsync(parameters.Route);

            Assert.Equal(parameters.ExpectedStatusCode, (int)response.StatusCode);
        }
    }
}
