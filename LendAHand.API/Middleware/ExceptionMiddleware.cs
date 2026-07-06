using LendAHand.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace LendAHand.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // 401/403 ko handle mat karo
                if (context.Response.StatusCode == 401 ||
                    context.Response.StatusCode == 403)
                {
                    return;
                }

                _logger.LogError(ex,
                    "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context, Exception exception)
        {
            // Agar response already started ho
            if (context.Response.HasStarted)
                return;

            context.Response.ContentType = "application/json";

            var statusCode = GetStatusCode(exception);
            context.Response.StatusCode = statusCode;

            var response = new
            {
                success = false,
                message = exception.Message,
                statusCode = statusCode
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                NotFoundException =>
                    (int)HttpStatusCode.NotFound,
                ValidationException =>
                    (int)HttpStatusCode.BadRequest,
                UnauthorizedException =>
                    (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };
    }
}