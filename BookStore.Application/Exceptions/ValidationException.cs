using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class ValidationException : BaseException
    {
        public List<string> Errors { get; }

        public ValidationException(string message) : base("VALIDATION", message)
        {
            Errors = new List<string> { message };
        }

        public ValidationException(List<string> errors) : base("VALIDATION", "Validation failed")
        {
            Errors = errors;
        }

    }
}
