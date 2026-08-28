using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base("NOT_FOUND", message) { }
        public NotFoundException(string entityName, object id)
            : base("NOT_FOUND", $"{entityName} with id '{id}' not found") { }
    }
}
