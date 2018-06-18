using AspNetCoreDemo.DependencyInjectionDemo.Contracts;
using System.Diagnostics;

namespace AspNetCoreDemo.DependencyInjectionDemo.Services
{
    internal class ServiceImplementationA : IExampleService
    {
        public ServiceImplementationA()
        {
            Debug.WriteLine($"{nameof(ServiceImplementationA)} created");
        }

        public void Execute()
        {
            Debug.WriteLine($"{nameof(ServiceImplementationA)} invoked");
        }
    }
}
