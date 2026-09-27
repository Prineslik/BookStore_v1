using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Exceptions
{
    public class DbConcurrencyException: BaseException
    {
        public DbConcurrencyException(string message) : base("DATABASE_CONCURRENCY_ERROR", message) { }

    }
}
