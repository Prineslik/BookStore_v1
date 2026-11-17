using BookStore.API.Extensions;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Exceptions;
using BookStore.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace BookStore.API.Helpers;

public static class ProblemDetailsFactory
{
    public static ProblemDetails FromException(HttpContext context, Exception ex)
    {
        if (ex is BaseException baseEx)
            return Build(
                context,
                baseEx.ErrorType.ToStatusCode(),
                baseEx.ErrorType.ToTitle(),
                baseEx.Message,
                "application");

        if (ex is FluentValidation.ValidationException validationEx)
        {
            var validationErrors = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return new ValidationProblemDetails(validationErrors) 
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "FluentValidationException",
                Detail = "One or more validation errors occurred.",
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = context.TraceIdentifier,
                    ["category"] = "API"
                }
            };
            //Build(
            //context,
            //StatusCodes.Status400BadRequest,
            //"One or more validation errors occurred.",
            //validationEx.Message,
            //"application");
        }


        int status;
        string title, detail;
        if (ex is DomainException domainEx)
        {
            (status, title, detail) = MapDomain(ex);

            return Build(
                context,
                status,
                title,
                detail,
                "domain");
        }

        (status, title, detail) = MapInfrastructure(ex);
        return Build(
            context,
            status,
            title,
            detail,
            "infrastructure");
    }
    
    public static ProblemDetails FromError(HttpContext context, Error error)
    {
        return Build(
            context,
            error.Type.ToStatusCode(),
            error.Type.ToTitle(),
            error.Message,
            "application");
    }

    private static (int Status, string Title, string Detail) MapInfrastructure(Exception ex) => ex switch
    {
        DbUpdateConcurrencyException => (409, "Concurrent data access violation",
            "The resource was modified by another user. Reload and try again."),
        DbUpdateException => (409, "Data conflict",
            "The operation could not be completed due to a data conflict."),
        DbException => (503, "Service unavailable",
            "The service is temporarily unavailable. Try again later."),
        _ => (500, "Internal server error", "An unexpected error occurred.")
    };

    private static (int Status, string Title, string Detail) MapDomain(Exception ex) => ex switch
    {
        ValidationException => (400, "Validation error",
            ex.Message),
        _ => (500, "Internal server error", "An unexpected error occurred.")
    };

    private static ProblemDetails Build(HttpContext context, int status, string title, string detail, string category)
        => new()
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Extensions =
            {
                ["traceId"] = context.TraceIdentifier,
                ["category"] = category
            }
        };
}
