using AspNetCoreDemo.HttpClientFactoryDemo.Model;
using System.Threading.Tasks;

namespace AspNetCoreDemo.HttpClientFactoryDemo.Contracts
{
    public interface IWikipediaClient
    {
        Task<PageInfo> GetInfoAsync(string titles);
    }
}
