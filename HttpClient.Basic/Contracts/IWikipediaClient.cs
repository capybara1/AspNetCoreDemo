using AspNetCoreDemo.HttpClient.Basic.Model;
using System.Threading.Tasks;

namespace AspNetCoreDemo.HttpClient.Basic.Contracts
{
    public interface IWikipediaClient
    {
        Task<PageInfo> GetInfoAsync(string titles);
    }
}
