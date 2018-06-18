using AspNetCoreDemo.DependencyInjectionDemo.Contracts;
using System;
using System.Diagnostics;

namespace AspNetCoreDemo.DependencyInjectionDemo.Services
{
    internal class OtherServiceImplementation : IOtherExampleService, IDisposable
    {
        public OtherServiceImplementation()
        {
            Debug.WriteLine($"{nameof(OtherServiceImplementation)} created");
        }

        public void Execute()
        {
            Debug.WriteLine($"{nameof(OtherServiceImplementation)} invoked");
        }

        public void Dispose()
        {
            Debug.WriteLine($"{nameof(OtherServiceImplementation)} disposed");
        }
    }
}
