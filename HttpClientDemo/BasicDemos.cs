using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.HttpClientFactoryDemo
{
    [Trait("Category", "HttpClient / Basics")]
    public class BasicDemos
    {
        // See https://www.stevejgordon.co.uk/introduction-to-httpclientfactory-aspnetcore

        private readonly ITestOutputHelper _testOutputHelper;

        public BasicDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Simple GET of string")]
        public async Task SimpleGetString()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php");

            var result = await client.GetStringAsync("?action=query&prop=info&titles=Main%20Page&format=json");

            _testOutputHelper.WriteLine($"Text: {result}");
        }

        [Fact(DisplayName = "Simple GET of any media type")]
        public async Task SimpleGet()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php");

            var response = await client.GetAsync("?action=query&prop=info&titles=Main%20Page&format=json");
            
            Assert.Equal(200, (int)response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine($"Text: {result}");
        }

        [Fact(DisplayName = "Simple GET with default header")]
        public async Task SimpleGetWithDefaultHeader()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://en.wikipedia.org/w/api.php");

            var userAgentHeaderValue = new ProductInfoHeaderValue("DemoTool", "1.0");
            client.DefaultRequestHeaders.UserAgent.Add(userAgentHeaderValue);

            var response = await client.GetAsync("?action=query&prop=info&titles=Main%20Page&format=json");

            Assert.Equal(200, (int)response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine($"Text: {result}");
        }

        [Fact(DisplayName = "GET with header")]
        public async Task GetWithHeader()
        {
            var client = new HttpClient();
            
            var requestUri = new Uri("https://en.wikipedia.org/w/api.php?action=query&prop=info&titles=Main%20Page?action=query&prop=info&titles=Main%20Page&format=json");
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var response = await client.SendAsync(request);

            Assert.Equal(200, (int)response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine($"Text: {result}");
        }
    }
}
