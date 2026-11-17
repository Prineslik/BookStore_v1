using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Roles.Commands
{
    public record RoleUpdateCommand(
        Guid Id,
        string Name,
        IReadOnlyCollection<Guid> PermissionIds);
}
