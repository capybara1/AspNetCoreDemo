﻿using AspNetCoreDemo.Middleware.Authorization.Contracts;
using AspNetCoreDemo.Middleware.Authorization.Security.Authentication;
using AspNetCoreDemo.Middleware.Authorization.Security.Authorization;
using AspNetCoreDemo.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.Middleware.Authorization
{
    [Trait("Category", "ASP.NET Core Middleware / Authorization")]
    public class AuthorizationDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AuthorizationDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }
        
        [Fact(DisplayName = "Use unspecific imperative authorization")]
        public async Task UseUnspecificImperativeAuthorization()
        {
            var requirements = new[]
            {
                new DenyAnonymousAuthorizationRequirement(),
            };
            await RunTestAsync(requirements);
        }

        [Fact(DisplayName = "Use imperative authorization with assertions")]
        public async Task UseImperativeAuthorizationWithAssertions()
        {
            var requirements = new[]
            {
                new AssertionRequirement(c => c.User.Identity.AuthenticationType == "DemoAuthenticationType"),
            };
            await RunTestAsync(requirements);
        }

        [Fact(DisplayName = "Use imperative authorization with claims")]
        public async Task UseImperativeAuthorizationWithClaims()
        {
            var requirements = new[]
            {
                new ClaimsAuthorizationRequirement(ClaimTypes.Email, new[] { "john.doe@example.com" }),
            };
            await RunTestAsync(requirements);
        }

        [Fact(DisplayName = "Use imperative authorization by name")]
        public async Task UseImperativeAuthorizationWithName()
        {
            var requirements = new[]
            {
                new NameAuthorizationRequirement("John Doe"),
            };
            await RunTestAsync(requirements);
        }

        [Fact(DisplayName = "Use imperative authorization with set of role alternatives")]
        public async Task UseImperativeAuthorizationWithSetOfRoleAlternatives()
        {
            var requirements = new[]
            {
                new RolesAuthorizationRequirement(new[] { "Contributor", "Admin" }),
            };
            await RunTestAsync(requirements);
        }

        [Fact(DisplayName = "Use imperative authorization with required set of roles")]
        public async Task UseImperativeAuthorizationWithSetOfRequiredRoles()
        {
            var requirements = new[]
            {
                new RolesAuthorizationRequirement(new[] { "Contributor" }),
                new RolesAuthorizationRequirement(new[] { "Admin" }),
            };
            await RunTestAsync(requirements, StatusCodes.Status403Forbidden);
        }

        [Fact(DisplayName = "Use imperative authorization with policy based on standard requirements")]
        public async Task UseImperativeAuthorizationWithPolicyBasedOnStandardRequirements()
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
                    services.AddAuthentication()
                        .AddScheme<DemoAuthenticationSchemeOptions, DemoAuthenticationHandler>("DemoScheme", opt => { });
                    services.AddAuthorization(opt => opt.AddPolicy(
                            "DemoPolicy",
                            policyBuilder => policyBuilder
                                .RequireAuthenticatedUser()
                                .RequireClaim(ClaimTypes.Email, new[] { "john.doe@example.com" })
                                .RequireRole("Contributor")));
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
                    app.Use(async (context, next) =>
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<AuthorizationDemos>>();
                        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
                        
                        var result = await authorizationService.AuthorizeAsync(
                            context.User,
                            null,
                            "DemoPolicy");

                        if (result.Succeeded)
                        {
                            await next();
                        }
                        else
                        {
                            foreach (var requirement in result.Failure.FailedRequirements)
                            {
                                logger.LogInformation($"Failed requirement: {requirement.GetType().Name}");
                            }
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

            client.DefaultRequestHeaders.Add("Proof", "Sound Proof");
            var response = await client.GetAsync("/");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }
        
        [Fact(DisplayName = "Use imperative authorization with policy based on custom requirement")]
        public async Task UseImperativeAuthorizationWithPolicyBasedOnCustomRequirement()
        {
            var userAuthorizationServiceMock = new Mock<IUserAuthorizationService>();
            userAuthorizationServiceMock
                .Setup(m => m.IsMethodAllowedForUser(It.IsAny<ClaimsPrincipal>()))
                .Returns(true);

            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                    services.AddSingleton(userAuthorizationServiceMock.Object);
                    services.AddSingleton<IAuthorizationHandler, MethodAllowedForUserAuthorizationHandler>();
                    services.AddAuthentication()
                        .AddScheme<DemoAuthenticationSchemeOptions, DemoAuthenticationHandler>("DemoScheme", opt => { });
                    services.AddAuthorization(opt => opt.AddPolicy(
                            "DemoPolicy",
                            policyBuilder => policyBuilder
                                .AddRequirements(new MethodAllowedForUserAuthorizationRequirement())));
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
                    app.Use(async (context, next) =>
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<AuthorizationDemos>>();
                        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
                        
                        var result = await authorizationService.AuthorizeAsync(
                            context.User,
                            null,
                            "DemoPolicy");

                        if (result.Succeeded)
                        {
                            await next();
                        }
                        else
                        {
                            foreach (var requirement in result.Failure.FailedRequirements)
                            {
                                logger.LogInformation($"Failed requirement: {requirement.GetType().Name}");
                            }
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

            client.DefaultRequestHeaders.Add("Proof", "Sound Proof");
            var response = await client.GetAsync("/");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact(DisplayName = "Use imperative authorization with resource")]
        public async Task UseImperativeAuthorizationWithResource()
        {
            var resourceRepositoryMock = new Mock<IResourceStore>();
            resourceRepositoryMock.Setup(m => m.GetResource("97dc6def"))
                .Returns(new Resource { Owner = new UserId("John Doe") });
            
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                    services.AddSingleton(resourceRepositoryMock.Object);
                    services.AddSingleton<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();
                    services.AddAuthentication()
                        .AddScheme<DemoAuthenticationSchemeOptions, DemoAuthenticationHandler>("DemoScheme", opt => { });
                    services.AddAuthorization();
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
                    app.Use(async (context, next) =>
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<AuthorizationDemos>>();
                        var repository = serviceProvider.GetRequiredService<IResourceStore>();
                        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

                        var resource = repository.GetResource("97dc6def");

                        var requirements = new[]
                        {
                            new ResourceOwnerAuthorizationRequirement(),
                        };
                        var result = await authorizationService.AuthorizeAsync(
                            context.User,
                            resource,
                            requirements);

                        if (result.Succeeded)
                        {
                            await next();
                        }
                        else
                        {
                            foreach (var requirement in result.Failure.FailedRequirements)
                            {
                                logger.LogInformation($"Failed requirement: {requirement.GetType().Name}");
                            }
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

            client.DefaultRequestHeaders.Add("Proof", "Sound Proof");
            var response = await client.GetAsync("/");

            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        private async Task RunTestAsync(IEnumerable<IAuthorizationRequirement> requirements, int expectedStatusCode = StatusCodes.Status200OK)
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(setup =>
                {
                    setup.AddDebug();
                    setup.SetupDemoLogging(_testOutputHelper);
                })
                .ConfigureServices(services =>
                {
                    services.AddAuthentication()
                        .AddScheme<DemoAuthenticationSchemeOptions, DemoAuthenticationHandler>("DemoScheme", opt => { });
                    services.AddAuthorization();
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
                    app.Use(async (context, next) =>
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<AuthorizationDemos>>();
                        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
                        
                        var result = await authorizationService.AuthorizeAsync(
                            context.User,
                            null,
                            requirements);

                        if (result.Succeeded)
                        {
                            await next();
                        }
                        else
                        {
                            foreach (var requirement in result.Failure.FailedRequirements)
                            {
                                logger.LogInformation($"Failed requirement: {requirement.GetType().Name}");
                            }
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

            client.DefaultRequestHeaders.Add("Proof", "Sound Proof");
            var response = await client.GetAsync("/");

            Assert.Equal(expectedStatusCode, (int)response.StatusCode);
        }
    }
}
