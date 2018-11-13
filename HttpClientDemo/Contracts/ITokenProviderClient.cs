using System.Threading.Tasks;

namespace AspNetCoreDemo.HttpClientFactoryDemo.Contracts
{
    public interface ITokenProviderClient
    {
        Task<string> GetTokenAsync();
    }
}
