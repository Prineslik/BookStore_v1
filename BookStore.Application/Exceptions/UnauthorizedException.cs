using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message) : base("UNATTHORIZED", message) { }
    }
}
