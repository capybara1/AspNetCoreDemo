namespace AspNetCoreDemo.HttpClient.Basic.Contracts
{
    public interface ITokenCache
    {
        void StoreToken(string token);
        bool TryGetToken(out string token);
    }
}
