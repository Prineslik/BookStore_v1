using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class BaseException : Exception
    {
        public string Code { get; }

        public BaseException(string code, string message) : base(message)
        {
            Code = code;
        }
    }
}
