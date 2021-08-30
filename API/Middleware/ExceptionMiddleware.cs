using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    // public static class ExceptionMiddlewareExtension
    // {
    //     public static IApplicationBuilder ApplyExceptionMiddleware(this IApplicationBuilder app)
    //     {
    //         app.UseMiddleware<ExceptionMiddleware>();
    //         return app;
    //     }
    // }
    public class ExceptionMiddleware
    {
        //request deleget is what is next in middleware pipe line 
        //ilogger to log out the error in terminal where dotnet run
        //environment host is to know if its development or production
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;


        //http context becouse this is happening in the http request and we have access
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var Response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(Response, options);

                await context.Response.WriteAsync(json);
            }

        }
    }
}