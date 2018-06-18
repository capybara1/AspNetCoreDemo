using AspNetCoreDemo.DependencyInjectionDemo.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCoreDemo.DependencyInjectionDemo
{
    public class ComplexDemos
    {
        private static void ResolveAndExecuteExampleService(IServiceProvider serviceProvider)
        {
            var implementation = serviceProvider.GetService<IExampleService>();
            implementation.Execute();
        }
    }
}
