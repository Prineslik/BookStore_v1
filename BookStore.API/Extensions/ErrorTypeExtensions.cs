using BookStore.Application.Contracts.Common;

namespace BookStore.API.Extensions
{
    public static class ErrorTypeExtensions
    {
        public static int ToStatusCode(this ErrorType errorType) => errorType switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Duplicate => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        public static string ToTitle(this ErrorType errorType) => errorType switch
        {
            ErrorType.NotFound => "Resource not found",
            ErrorType.Validation => "Validation error",
            ErrorType.Conflict => "Conflict",
            ErrorType.Forbidden => "Access denied",
            ErrorType.Unauthorized => "User unauthorized",
            ErrorType.Duplicate => "Diplicate",
            _ => "Internal server error"
        };
    }
}
