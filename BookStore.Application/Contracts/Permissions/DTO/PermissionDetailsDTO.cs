using BookStore.Application.Contracts.Roles.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Permissions.DTO
{
    public record PermissionDetailsDTO(
        Guid Id,
        string Code,
        string Description,
        List<RoleShortDTO> Roles);
}
