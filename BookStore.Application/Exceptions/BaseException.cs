using BookStore.Application.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public abstract class BaseException : Exception
    {
        public abstract ErrorType ErrorType { get; }

        protected BaseException(string message) : base(message) { }
        protected BaseException(string message, Exception inner) : base(message, inner) { }
    }
}
