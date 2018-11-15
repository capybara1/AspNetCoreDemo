using AspNetCoreDemo.HttpClient.Polly.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http.Headers;
using Xunit;

namespace AspNetCoreDemo.HttpClient.Polly
{
    [Trait("Category", "HttpClient / Policy Handler")]
    public class PolicyHandlerDemos
    {
        // https://www.stevejgordon.co.uk/httpclientfactory-using-polly-for-transient-fault-handling
        // https://docs.microsoft.com/de-de/dotnet/standard/microservices-architecture/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly

        private static readonly Random Jitterer = new Random();

        [Fact(DisplayName = "Add typed client with policy handler")]
        public void AddTypedClientWithPolicyHandler()
        {
            var policy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, GetDuration);

            var services = new ServiceCollection();

            services.AddHttpClient<IWikipediaClient, WikipediaClient>(c =>
                {
                    c.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php");
                    var userAgentHeaderValue = new ProductInfoHeaderValue("DemoTool", "1.0");
                    c.DefaultRequestHeaders.UserAgent.Add(userAgentHeaderValue);
                })
                .AddPolicyHandler(policy);

            var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetService<IWikipediaClient>();

            Assert.NotNull(client);
        }

        [Fact(DisplayName = "Add typed client with transient error policy handler")]
        public void AddTypedClientWithTransientErrorPolicyHandler()
        {
            var services = new ServiceCollection();

            services.AddHttpClient<IWikipediaClient, WikipediaClient>(c =>
                {
                    c.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php");
                    var userAgentHeaderValue = new ProductInfoHeaderValue("DemoTool", "1.0");
                    c.DefaultRequestHeaders.UserAgent.Add(userAgentHeaderValue);
                })
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetry(3, GetDuration));

            var serviceProvider = services.BuildServiceProvider();

            var client = serviceProvider.GetService<IWikipediaClient>();

            Assert.NotNull(client);
        }

        private TimeSpan GetDuration(int retryAttempt)
        {
            var baseDuration = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
            var jitter = TimeSpan.FromMilliseconds(Jitterer.Next(0, 100));
            var result = baseDuration + jitter;

            return result;
        }
    }
}
