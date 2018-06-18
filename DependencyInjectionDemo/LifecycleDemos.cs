using AspNetCoreDemo.DependencyInjectionDemo.Contracts;
using AspNetCoreDemo.DependencyInjectionDemo.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.DependencyInjectionDemo
{
    [Trait("Category", "Dependency Injection / Lifecycle")]
    public class LifecycleDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public LifecycleDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Singleton by Type")]
        public void ConfigureSingletonByType()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IExampleService, ServiceImplementationB>();

            var serviceProvider = services.BuildServiceProvider();
            ResolveAndExecuteExampleService(serviceProvider);
            ResolveAndExecuteExampleService(serviceProvider);

            using (var scope = serviceProvider.CreateScope())
            {
                ResolveAndExecuteExampleService(scope.ServiceProvider);
                ResolveAndExecuteExampleService(scope.ServiceProvider);
            }
        }

        [Fact(DisplayName = "Singleton by Instance")]
        public void ConfigureSingletonByInstance()
        {
            var services = new ServiceCollection();

            var instance = new ServiceImplementationB();
            services.AddSingleton<IExampleService>(instance);

            var serviceProvider = services.BuildServiceProvider();
            ResolveAndExecuteExampleService(serviceProvider);
            ResolveAndExecuteExampleService(serviceProvider);

            using (var scope = serviceProvider.CreateScope())
            {
                ResolveAndExecuteExampleService(scope.ServiceProvider);
                ResolveAndExecuteExampleService(scope.ServiceProvider);
            }
        }

        [Fact(DisplayName = "Scoped by Type")]
        public void ConfigureScopedByType()
        {
            var services = new ServiceCollection();

            services.AddScoped<IExampleService, ServiceImplementationB>();

            var serviceProvider = services.BuildServiceProvider();
            ResolveAndExecuteExampleService(serviceProvider);
            ResolveAndExecuteExampleService(serviceProvider);

            using (var scope = serviceProvider.CreateScope())
            {
                ResolveAndExecuteExampleService(scope.ServiceProvider);
                ResolveAndExecuteExampleService(scope.ServiceProvider);
            }
        }

        [Fact(DisplayName = "Scoped by Factory")]
        public void ConfigureScopedByFactory()
        {
            var services = new ServiceCollection();

            services.AddScoped<IExampleService>(sp => new ServiceImplementationB());

            var serviceProvider = services.BuildServiceProvider();
            ResolveAndExecuteExampleService(serviceProvider);
            ResolveAndExecuteExampleService(serviceProvider);

            using (var scope = serviceProvider.CreateScope())
            {
                ResolveAndExecuteExampleService(scope.ServiceProvider);
                ResolveAndExecuteExampleService(scope.ServiceProvider);
            }
        }

        [Fact(DisplayName = "Transient by Type")]
        public void ConfigureTransientByType()
        {
            var services = new ServiceCollection();

            services.AddTransient<IExampleService, ServiceImplementationB>();

            var serviceProvider = services.BuildServiceProvider();
            ResolveAndExecuteExampleService(serviceProvider);
            ResolveAndExecuteExampleService(serviceProvider);

            using (var scope = serviceProvider.CreateScope())
            {
                ResolveAndExecuteExampleService(scope.ServiceProvider);
                ResolveAndExecuteExampleService(scope.ServiceProvider);
            }
        }

        [Fact(DisplayName = "Transient by Factory")]
        public void ConfigureTransient()
        {
            var services = new ServiceCollection();

            services.AddTransient<IExampleService>(sp => new ServiceImplementationB());

            var serviceProvider = services.BuildServiceProvider();
            ResolveAndExecuteExampleService(serviceProvider);
            ResolveAndExecuteExampleService(serviceProvider);

            using (var scope = serviceProvider.CreateScope())
            {
                ResolveAndExecuteExampleService(scope.ServiceProvider);
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
