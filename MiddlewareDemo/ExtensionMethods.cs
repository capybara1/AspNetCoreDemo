using AspNetCoreDemo.MiddlewareDemo.Contracts;
using AspNetCoreDemo.MiddlewareDemo.Implementations;
using AspNetCoreDemo.MiddlewareDemo.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreDemo.MiddlewareDemo
{
    public static class ExtensionMethods
    {
        public static void AddExample(this IServiceCollection services)
        {
            services.AddTransient<IExampleService, ServiceImplementationA>();
            services.AddTransient<ExampleMiddlewareWithDependency>();
        }

        public static void UseExample(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExampleMiddlewareWithDependency>();
        }
    }
}
