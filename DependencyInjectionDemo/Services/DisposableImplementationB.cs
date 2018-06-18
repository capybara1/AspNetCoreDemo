using AspNetCoreDemo.DependencyInjectionDemo.Contracts;
using System;
using System.Diagnostics;

namespace AspNetCoreDemo.DependencyInjectionDemo.Services
{
    internal class DisposableImplementationB : IExampleService, IDisposable
    {
        public DisposableImplementationB()
        {
            Debug.WriteLine($"{nameof(DisposableImplementationB)} created");
        }

        public void Execute()
        {
            Debug.WriteLine($"{nameof(DisposableImplementationB)} invoked");
        }

        public void Dispose()
        {
            Debug.WriteLine($"{nameof(DisposableImplementationB)} disposed");
        }
    }
}
