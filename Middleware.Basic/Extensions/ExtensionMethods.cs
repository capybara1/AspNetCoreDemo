using AspNetCoreDemo.Middleware.Basic.Contracts;
using AspNetCoreDemo.Middleware.Basic.Services;
using AspNetCoreDemo.Middleware.Basic.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreDemo.Middleware.Basic
{
    public static class ExtensionMethods
    {
        public static void AddExample(this IServiceCollection services)
        {
            services.AddTransient<IExampleService, ExampleService>();
            services.AddTransient<ExampleMiddlewareWithDependency>();
        }

        public static void UseExample(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExampleMiddlewareWithDependency>();
        }
    }
}
