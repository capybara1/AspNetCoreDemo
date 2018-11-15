using System.IO;

namespace AspNetCoreDemo.Middleware.Basic.Contracts
{
    public interface ISerializer
    {
        string SerializeObject(object obj, string mediaType);
        object DeserializeObject(Stream stream, string mediaType);
    }
}
