using System.Threading.Tasks;

namespace AspNetCoreDemo.Mvc.Basic.Contracts
{
    public interface IUserInfoService
    {
        Task<bool> IsUserAsync(string username, string password);
    }
}
