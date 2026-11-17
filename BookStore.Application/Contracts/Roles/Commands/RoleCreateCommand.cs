using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Roles.Commands
{
    public record RoleCreateCommand(
        string Name,
        IReadOnlyCollection<Guid> PermissionIds
    );
}
