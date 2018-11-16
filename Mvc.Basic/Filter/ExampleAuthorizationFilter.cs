using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace AspNetCoreDemo.Mvc.Basic.Filter
{
    public class ExampleAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Debug.WriteLine($"In {nameof(ExampleAuthorizationFilter)}");
        }
    }
}
