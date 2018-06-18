using System.Threading.Tasks;

namespace AspNetCoreDemo.MvcDemo.Contracts
{
    public interface IUserInfoService
    {
        Task<bool> IsUserAsync(string username, string password);
        Task<bool> IsAdminAsync(string username);
    }
}
