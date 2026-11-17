using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Permissions.DTO
{
    public record PermissionShortDTO(
        Guid Id,
        string Code,
        string Description);
}
