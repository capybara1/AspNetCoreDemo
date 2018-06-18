using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCoreDemo.MvcDemo.Filter
{
    public class ExampleAsyncAuthorizationFilter : IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Debug.WriteLine($"In {nameof(ExampleAsyncAuthorizationFilter)}");
            return Task.CompletedTask;
        }
    }
}
