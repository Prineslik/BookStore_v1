using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class InfrastructureException : BaseException
    {
        public InfrastructureException(string message) : base("INFRASTRUCTURE_ERROR", message) { }
    }
}
