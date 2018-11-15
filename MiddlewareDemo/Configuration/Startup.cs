using AspNetCoreDemo.MiddlewareDemo.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreDemo.MiddlewareDemo.Configuration
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddExample();
        }

        public void Configure(
            IApplicationBuilder app,
            // Additional dependencies
            IExampleService service)
        {
            app.UseExample();

            service.Execute();
        }
    }
}
