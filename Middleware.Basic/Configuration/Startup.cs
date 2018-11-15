using AspNetCoreDemo.Middleware.Basic.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreDemo.Middleware.Basic.Configuration
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
