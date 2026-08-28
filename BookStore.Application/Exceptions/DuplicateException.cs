using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class DuplicateException : BaseException
    {
        public DuplicateException(string message) : base("DUPLICATE", message) { }
    }
}
