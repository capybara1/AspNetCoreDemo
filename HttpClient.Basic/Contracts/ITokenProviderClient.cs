using System.Threading.Tasks;

namespace AspNetCoreDemo.HttpClient.Basic.Contracts
{
    public interface ITokenProviderClient
    {
        Task<string> GetTokenAsync();
    }
}
