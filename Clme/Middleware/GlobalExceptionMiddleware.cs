using System.Net;
using System.Text.Json;

namespace Clme.Middleware
    {
    public class GlobalExceptionMiddleware
        {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
            {
            _next = next;
            _logger = logger;
            _env = env;
            }

        public async Task InvokeAsync(HttpContext context)
            {
            try
                {
                await _next(context); // Move to the next piece of middleware
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
                }
            }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // In Development, show the full stack trace; in Production, show a polite message.
            var response = new
                {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error. Our engineers are on it!",
                Detailed = _env.IsDevelopment() ? exception.ToString() : null
                };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }