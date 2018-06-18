using System.IO;

namespace AspNetCoreDemo.MiddlewareDemo.Contracts
{
    public interface ISerializer
    {
        string SerializeObject(object obj, string mediaType);
        object DeserializeObject(Stream stream, string mediaType);
    }
}
