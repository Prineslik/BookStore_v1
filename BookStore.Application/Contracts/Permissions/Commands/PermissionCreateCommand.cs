using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Permissions.Commands
{
    public record PermissionCreateCommand(
        string Code,
        string Description);
}
