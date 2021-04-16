# Custom Exception Page
* Add exception
* Add middleware
* Register middleware

### Add exception
in `Errors/ApiException.cs`
```cs
namespace test.Errors
{
    public class ApiException
    {
        public ApiException(string message, string details = null)
        {
            Message = message;
            Details = details;

        }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
```
### Add middleware
in `Middlewares/ExceptionMiddleware.cs`
```cs
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using test.Errors;
using System.Text.Json;

namespace test.Middleware
{
    public class ExceptionMiddleware
    {
        public readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;

                var response = _env.IsDevelopment()
                    ? new ApiException(exception.Message, exception.StackTrace?.ToString())
                    : new ApiException(exception.Message);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }   
        }

    }
}
```
### Register middleware
in `Startup.cs`
```cs
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // if (env.IsDevelopment())
    // {
    //     app.UseDeveloperExceptionPage();
    //     app.UseSwagger();
    //     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "test v1"));
    // }
    app.UseMiddleware<ExceptionMiddleware>();
    // ...
}
```
