using BookStore.Application.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public override ErrorType ErrorType => ErrorType.Unauthorized;
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, Exception inner) : base(message, inner) { }
    }
}
