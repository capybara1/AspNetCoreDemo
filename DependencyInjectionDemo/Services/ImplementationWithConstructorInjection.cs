using AspNetCoreDemo.DependencyInjectionDemo.Contracts;
using System;
using System.Diagnostics;

namespace AspNetCoreDemo.DependencyInjectionDemo.Services
{
    internal class ImplementationWithConstructorInjection : IExampleService, IDisposable
    {
        private readonly IOtherExampleService _otherService;
        private readonly string _testValue;

        public ImplementationWithConstructorInjection(IOtherExampleService otherService)
        {
            _otherService = otherService ?? throw new ArgumentNullException(nameof(otherService));

            Debug.WriteLine($"{nameof(ImplementationWithConstructorInjection)} created");
        }

        public ImplementationWithConstructorInjection(IOtherExampleService otherService, string testValue)
        {
            _otherService = otherService ?? throw new ArgumentNullException(nameof(otherService));
            _testValue = testValue ?? throw new ArgumentNullException(nameof(testValue));
            Debug.WriteLine($"{nameof(ImplementationWithConstructorInjection)} created");
        }

        public void Execute()
        {
            Debug.WriteLine($"{nameof(ImplementationWithConstructorInjection)} invoked");
            if (!string.IsNullOrEmpty(_testValue))
            {
                Debug.WriteLine(_testValue);
            }

            _otherService.Execute();
        }

        public void Dispose()
        {
            Debug.WriteLine($"{nameof(ImplementationWithConstructorInjection)} disposed");
        }
    }
}
