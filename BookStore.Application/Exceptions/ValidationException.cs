using BookStore.Application.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class ValidationException : BaseException
    {
        public override ErrorType ErrorType => ErrorType.Validation;
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }

    }
}
