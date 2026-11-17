using BookStore.API.Extensions;
using BookStore.API.Helpers;
using BookStore.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Net;
using System.Net.NetworkInformation;
using BookStore.Core.Exceptions;

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
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation("Request aborted by client: {Path} ({TraceId})",
                context.Request.Path, context.TraceIdentifier);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogError(ex, "Response already started; cannot write ProblemDetails ({TraceId})",
                        context.TraceIdentifier);
                    throw;
                }

                //var category = ex is BaseException ? "application" : "infrastructure";

                var problem = ProblemDetailsFactory.FromException(context, ex);
                problem.Extensions.TryGetValue("category", out var category);
                
                LogException(context,ex, problem, category.ToString());

                context.Response.Clear();
                context.Response.StatusCode = (int)problem.Status!;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problem);
            }
        }

        private void LogException(HttpContext context,Exception exception,ProblemDetails problem,string category)
        {
            if ((exception is BaseException || exception is DomainException || exception is FluentValidation.ValidationException) && problem.Status < 500)
            {
                _logger.LogWarning(
                    "Handled {Category} exception -> {StatusCode} {Title} at {Path}: {Detail} (TraceId={TraceId})",
                    category,
                    problem.Status,
                    problem.Title,
                    context.Request.Path,
                    problem.Detail,
                    context.TraceIdentifier);
                return;
            }

            if (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "Request cancelled by client at {Path} (TraceId={TraceId})",
                    context.Request.Path,
                    context.TraceIdentifier);
                return;
            }

            _logger.LogError(
                exception,
                "Unhandled {Category} exception -> {StatusCode} {Title} at {Path} (TraceId={TraceId})",
                category,
                problem.Status,
                problem.Title,
                context.Request.Path,
                context.TraceIdentifier);
        }
    }
}
