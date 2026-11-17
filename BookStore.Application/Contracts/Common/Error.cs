using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Common
{
    public sealed record Error
    {
        public static readonly Error None = new(ErrorType.None, string.Empty);

        public ErrorType Type { get; }
        public string Message { get; }

        public Error(ErrorType type, string message)
        {
            Type = type;
            Message = message;
        }

        public static Error NotFound(string entityName, string parameterName, object parameterValue) =>
            new(ErrorType.NotFound, $"{entityName} with {parameterName} {parameterValue} not found");
        
        public static Error Conflict(string entityName) =>
            new(ErrorType.Conflict, $"A conflict occurred with {entityName}");
        
        public static Error Validation(string message) =>
            new(ErrorType.Validation, message);

        public static Error Unauthorized(string message) =>
            new(ErrorType.Unauthorized, message);

        public static Error Forbidden(string message) =>
            new(ErrorType.Forbidden, message);

        public static Error Duplicate(string field, string value) =>
            new(ErrorType.Duplicate, $"{field} '{value}' already exists");
    }
}
