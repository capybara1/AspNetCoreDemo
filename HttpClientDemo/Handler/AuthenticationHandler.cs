using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreDemo.HttpClientFactoryDemo.Handler
{
    public class AuthenticationHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Proof", "A Sound Proof");

            return base.SendAsync(request, cancellationToken);
        }
    }
}