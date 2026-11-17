using BookStore.Application.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public override ErrorType ErrorType => ErrorType.Forbidden;
        public ForbiddenException(string message) : base(message) { }
        public ForbiddenException(string message, Exception inner) : base(message, inner) { }
    }
}
