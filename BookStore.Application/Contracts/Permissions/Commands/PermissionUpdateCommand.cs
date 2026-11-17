using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Permissions.Commands
{
    public record PermissionUpdateCommand(
        Guid Id,
        string Code,
        string Description);
}
