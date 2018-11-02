using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AspNetCoreDemo.ExternalTestLibrary.Startup))]

namespace AspNetCoreDemo.ExternalTestLibrary
{
    public class Startup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddTransient<IExternalService, ExternalService>();
            });
        }
    }
}
