using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Common
{
    public enum ErrorType 
    { 
        NotFound,
        Conflict,
        Validation,
        Unauthorized,
        Forbidden,
        Duplicate,
        None,

    }

}
