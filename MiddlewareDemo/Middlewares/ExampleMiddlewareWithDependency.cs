using AspNetCoreDemo.MiddlewareDemo.Contracts;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCoreDemo.MiddlewareDemo.Middlewares
{
    internal class ExampleMiddlewareWithDependency : IMiddleware
    {
        private readonly IExampleService _service;

        public ExampleMiddlewareWithDependency(IExampleService service)
        {
            _service = service ?? throw new System.ArgumentNullException(nameof(service));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path;
            var name = Regex.Match(path, @"(?<=^\/hello\/).*$").Value;

            _service.Execute();

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync($"Hello, {name}!", new UTF8Encoding(false));
        }
    }
}