using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Permissions
{
    public record PermissionsRequest(
        Guid? Id,
        string Code,
        string Description,
        IEnumerable<Guid?> RoleIds
    );
}
