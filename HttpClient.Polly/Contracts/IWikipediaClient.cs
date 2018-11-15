using AspNetCoreDemo.HttpClient.Polly.Model;
using System.Threading.Tasks;

namespace AspNetCoreDemo.HttpClient.Polly.Contracts
{
    public interface IWikipediaClient
    {
        Task<PageInfo> GetInfoAsync(string titles);
    }
}
