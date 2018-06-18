using AspNetCoreDemo.DependencyInjectionDemo.Contracts;
using AspNetCoreDemo.DependencyInjectionDemo.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace AspNetCoreDemo.DependencyInjectionDemo
{
    [Trait("Category", "Dependency Injection / Basics")]
    public class BasicDemos
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BasicDemos(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Fact(DisplayName = "Add Single / Resolve Single")]
        public void AddSingleImplementation_ResolveSingleService()
        {
            var services = new ServiceCollection();

            services.AddTransient<IExampleService, ServiceImplementationA>();

            var serviceProvider = services.BuildServiceProvider();

            var instance = serviceProvider.GetService<IExampleService>();

            instance.Execute();
        }

        [Fact(DisplayName = "Add Multiple / Resolve Single")]
        public void AddMultipleImplementations_ResolveSingleService()
        {
            var services = new ServiceCollection();

            services.AddTransient<IExampleService, ServiceImplementationA>();
            services.AddTransient<IExampleService, ServiceImplementationB>();

            var serviceProvider = services.BuildServiceProvider();

            var instance = serviceProvider.GetService<IExampleService>();

            instance.Execute();
        }

        [Fact(DisplayName = "Add Multiple / Resolve Multiple")]
        public void AddMultipleImplementations_ResolveMultipleServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<IExampleService, ServiceImplementationA>();
            services.AddTransient<IExampleService, ServiceImplementationB>();

            var serviceProvider = services.BuildServiceProvider();

            var instances = serviceProvider.GetService<IEnumerable<IExampleService>>();

            foreach (var instance in instances)
            {
                instance.Execute();
            }
        }

        [Fact(DisplayName = "Replace Implementation")]
        public void ReplaceImplementation()
        {
            var services = new ServiceCollection();

            services.AddTransient<IExampleService, ServiceImplementationA>();

            var serviceDescriptors = services
                .Where(sd => sd.ServiceType == typeof(IExampleService))
                .ToArray();

            foreach (var serviceDescriptor in serviceDescriptors)
            {
                services.Remove(serviceDescriptor);
            }

            services.AddTransient<IExampleService, ServiceImplementationB>();

            var serviceProvider = services.BuildServiceProvider();

            var instances = serviceProvider.GetService<IEnumerable<IExampleService>>();

            foreach (var instance in instances)
            {
                instance.Execute();
            }
        }

        [Fact(DisplayName = "Add None / Resolve Optional")]
        public void AddNoImplementation_ResolveOptionalServiceOptional()
        {
            var services = new ServiceCollection();

            var serviceProvider = services.BuildServiceProvider();

            var instance = serviceProvider.GetService<IExampleService>();

            Assert.Null(instance);
        }

        [Fact(DisplayName = "Add None / Resolve Required")]
        public void AddSingleImplementation_ResolveRequiredService()
        {
            var services = new ServiceCollection();

            var serviceProvider = services.BuildServiceProvider();

            Assert.Throws<InvalidOperationException>(() => serviceProvider.GetRequiredService<IExampleService>());
        }

        [Fact(DisplayName = "Add Dependency / Create Dependend")]
        public void AddSingleImplementation_CreateInstance()
        {
            var services = new ServiceCollection();

            services.AddTransient<IOtherExampleService, OtherServiceImplementation>();

            var serviceProvider = services.BuildServiceProvider();

            var instance = ActivatorUtilities.CreateInstance<ImplementationWithConstructorInjection>(serviceProvider);
            instance.Execute();
        }
    }
}
