using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Permissions
{
    public record PermissionsResponse(
        Guid Id,
        string Code,
        string Description
    );
    
}
