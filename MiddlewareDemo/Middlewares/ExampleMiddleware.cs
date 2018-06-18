﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Text;

namespace AspNetCoreDemo.MiddlewareDemo.Middlewares
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