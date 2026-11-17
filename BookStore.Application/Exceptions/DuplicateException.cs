using BookStore.Application.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class DuplicateException : BaseException
    {
        public override ErrorType ErrorType => ErrorType.Duplicate;

        public DuplicateException(string message) : base(message) { }
        public DuplicateException(string message, Exception inner) : base(message, inner) { }
    }
}
