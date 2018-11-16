using AspNetCoreDemo.HttpClient.Basic.Contracts;
using AspNetCoreDemo.HttpClient.Basic.Handler;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.HttpClient.Basic
{
    [Trait("Category", "HttpClient / HttpClientFactory")]
    public class HttpClientFactoryDemos
    {
        // See https://www.stevejgordon.co.uk/introduction-to-httpclientfactory-aspnetcore

        private readonly ITestOutputHelper _testOutputHelper;

        public HttpClientFactoryDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Add named client")]
        public void AddNamedClient()
        {
            var services = new ServiceCollection();

            services.AddHttpClient("DemoClient", c => // Requires Microsoft.Extensions.Http package
            {
                c.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php");
                var userAgentHeaderValue = new ProductInfoHeaderValue("DemoTool", "1.0");
                c.DefaultRequestHeaders.UserAgent.Add(userAgentHeaderValue);
            });

            var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetService<IHttpClientFactory>();

            var client = factory.CreateClient("DemoClient");

            Assert.NotNull(client);
        }

        [Fact(DisplayName = "Add typed client")]
        public void AddTypedClient()
        {
            var services = new ServiceCollection();

            services.AddHttpClient<IWikipediaClient, WikipediaClient>(c =>
                {
                    c.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php");
                    var userAgentHeaderValue = new ProductInfoHeaderValue("DemoTool", "1.0");
                    c.DefaultRequestHeaders.UserAgent.Add(userAgentHeaderValue);
                });

            var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetService<IWikipediaClient>();

            Assert.NotNull(client);
        }

        [Fact(DisplayName = "Add typed client with delegating handler")]
        public void AddTypedClientWithDelegatingHandler()
        {
            var tokenCacheMock = new Mock<ITokenCache>();
            var tokenProviderClientMock = new Mock<ITokenProviderClient>();

            tokenProviderClientMock
                .Setup(m => m.GetTokenAsync())
                .ReturnsAsync("Valid Token");

            var services = new ServiceCollection();

            services.AddSingleton(tokenCacheMock.Object);
            services.AddSingleton(tokenProviderClientMock.Object);
            services.AddScoped<AuthenticationHandler>();
            services.AddHttpClient<IWikipediaClient, WikipediaClient>(c =>
                {
                    c.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php");
                    var userAgentHeaderValue = new ProductInfoHeaderValue("DemoTool", "1.0");
                    c.DefaultRequestHeaders.UserAgent.Add(userAgentHeaderValue);
                })
                .AddHttpMessageHandler<AuthenticationHandler>();

            var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetService<IWikipediaClient>();

            Assert.NotNull(client);
        }
    }
}
