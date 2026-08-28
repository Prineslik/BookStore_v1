using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Roles
{
    public record RolesResponse(
        Guid Id,
        string Name,
        List<Guid?> PermissionIds
    );
}