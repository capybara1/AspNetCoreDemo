using AspNetCoreDemo.DependencyInjectionDemo.Contracts;
using System.Diagnostics;

namespace AspNetCoreDemo.DependencyInjectionDemo.Services
{
    internal class ServiceImplementationB : IExampleService
    {
        public ServiceImplementationB()
        {
            Debug.WriteLine($"{nameof(ServiceImplementationB)} created");
        }

        public void Execute()
        {
            Debug.WriteLine($"{nameof(ServiceImplementationB)} invoked");
        }
    }
}
