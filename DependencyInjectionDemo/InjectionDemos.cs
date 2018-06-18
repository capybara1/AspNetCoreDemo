using AspNetCoreDemo.DependencyInjectionDemo.Contracts;
using AspNetCoreDemo.DependencyInjectionDemo.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.DependencyInjectionDemo
{
    [Trait("Category", "Dependency Injection / Injection")]
    public class InjectionDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public InjectionDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Constructor Injection (Service Configured)")]
        public void ConstructorInjection_ServiceConfigured()
        {
            var services = new ServiceCollection();

            services.AddScoped<IExampleService, ImplementationWithConstructorInjection>();
            services.AddScoped<IOtherExampleService, OtherServiceImplementation>();

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                ResolveAndExecuteExampleService(scope.ServiceProvider);
            }
        }

        [Fact(DisplayName = "Constructor Injection (Service Not Configured)")]
        public void ConstructorInjection_ServiceNotConfigured()
        {
            var services = new ServiceCollection();

            services.AddScoped<IExampleService, ImplementationWithConstructorInjection>();

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                Assert.Throws<InvalidOperationException>(() => ResolveAndExecuteExampleService(scope.ServiceProvider));
            }
        }
        
        [Fact(DisplayName = "Explicit Construction with Factory")]
        public void ConstructionWithFactory()
        {
            var services = new ServiceCollection();

            services.AddScoped<IExampleService>(sp =>
            {
                var otherService = sp.GetRequiredService<IOtherExampleService>();
                var result = new ImplementationWithConstructorInjection(otherService);
                return result;
            });

            services.AddScoped<IOtherExampleService, OtherServiceImplementation>();

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                ResolveAndExecuteExampleService(scope.ServiceProvider);
            }
        }

        private static void ResolveAndExecuteExampleService(IServiceProvider serviceProvider)
        {
            var implementation = serviceProvider.GetService<IExampleService>();
            implementation.Execute();
        }
    }
}
