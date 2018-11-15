using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Middleware.Basic.Middlewares
{
    internal class ExampleMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path;
            var name = Regex.Match(path, @"(?<=^\/hello\/).*$").Value;

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync($"Hello, {name}!", new UTF8Encoding(false));
        }
    }
}