using System.Collections.Generic;

namespace AspNetCoreDemo.Middleware.Authorization.Contracts
{
    public interface IResourceStore
    {
        IEnumerable<Resource> GetResourceCollection();
        object GetResource(string key);
        string AddResource(object resource);
        bool StoreResource(string key, object resource);
        void RemoveResource(string key);
        void Clear();
        object GenerateId();
    }
}
