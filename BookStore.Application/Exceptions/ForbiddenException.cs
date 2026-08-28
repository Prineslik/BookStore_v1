using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message) : base("FORBIDDEN", message) { }
    }
}
