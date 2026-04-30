using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Common
{
    public record Error
    {
        public static readonly Error None = new(string.Empty, string.Empty);

        public string Code { get; }
        public string Message { get; }

        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static Error NotFound(string entityName, string parameterName, object parameterValue) =>
            new("NOT_FOUND", $"{entityName} with {parameterName} {parameterValue} not found");

        public static Error ListIsEmpty(string listName) =>
            new("LIST_IS_EMPTY", $"The list of {listName} does not contain any objects");

        public static Error Duplicate(string field, string value) =>
            new("DUPLICATE", $"{field} '{value}' already exists");

        public static Error Validation(string message) =>
            new("VALIDATION", message);

        public static Error Unexpected(string message) =>
            new("UNEXPECTED", message);
    }
}
