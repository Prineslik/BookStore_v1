using BookStore.Application.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public override ErrorType ErrorType => ErrorType.NotFound;
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
