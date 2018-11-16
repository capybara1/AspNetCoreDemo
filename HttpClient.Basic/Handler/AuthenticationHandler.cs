using AspNetCoreDemo.HttpClient.Basic.Contracts;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreDemo.HttpClient.Basic.Handler
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly ITokenCache _tokenCache;
        private readonly ITokenProviderClient _tokenProviderClient;

        public AuthenticationHandler(
            ITokenCache tokenCache,
            ITokenProviderClient tokenProviderClient)
        {
            _tokenCache = tokenCache ?? throw new System.ArgumentNullException(nameof(tokenCache));
            _tokenProviderClient = tokenProviderClient ?? throw new System.ArgumentNullException(nameof(tokenProviderClient));
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!_tokenCache.TryGetToken(out var token))
            {
                token = await _tokenProviderClient.GetTokenAsync();
                _tokenCache.StoreToken(token);
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var retryCount = 0;
            HttpResponseMessage response;
            do
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            while (response.StatusCode == HttpStatusCode.Unauthorized && retryCount++ < 1);
            
            return response;
        }
    }
}