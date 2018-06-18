using AspNetCoreDemo.MvcDemo.Contracts;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCoreDemo.MvcDemo.Filter
{
    public class ExampleAsyncActionFilterWithDependencyInjection : IAsyncActionFilter
    {
        private readonly IExampleService _exampleService;

        public ExampleAsyncActionFilterWithDependencyInjection(IExampleService exampleService)
        {
            _exampleService = exampleService ?? throw new System.ArgumentNullException(nameof(exampleService));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _exampleService.Execute();
            Debug.WriteLine($"In pre action code of {nameof(ExampleAsyncActionFilter)}");
            await next();
            Debug.WriteLine($"In post action code of {nameof(ExampleAsyncActionFilter)}");
        }
    }
}
