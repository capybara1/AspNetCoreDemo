using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace AspNetCoreDemo.Mvc.Basic.Filter
{
    public class ExampleResourceFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            Debug.WriteLine($"In pre action code of {nameof(ExampleResourceFilter)}");
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            Debug.WriteLine($"In post action code of {nameof(ExampleResourceFilter)}");
        }
    }
}
