using BookStore.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookStore.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            var (statusCode, title, detail) = exception switch
            {
                NotFoundException ex => (HttpStatusCode.NotFound, "Resource not found", ex.Message),
                ValidationException ex => (HttpStatusCode.BadRequest, "Validation error", ex.Message),
                DuplicateException ex => (HttpStatusCode.Conflict, "Duplicate resource", ex.Message),
                ForbiddenException ex => (HttpStatusCode.Forbidden, "Access denied", ex.Message),
                UnauthorizedException ex => (HttpStatusCode.Unauthorized, "User unauthorized", ex.Message),
                _ => (HttpStatusCode.InternalServerError, "Internal server error", "An unexpected error occurred")
            };

            context.Response.StatusCode = (int)statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Extensions = {
                ["traceId"] = context.TraceIdentifier
            }};

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
