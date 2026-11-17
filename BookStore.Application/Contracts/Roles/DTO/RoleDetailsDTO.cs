using BookStore.Application.Contracts.Permissions.DTO;
using BookStore.Application.Contracts.Users.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Roles.DTO
{
    public record RoleDetailsDTO(
        Guid Id,
        string Name,
        List<PermissionShortDTO> Permissions,
        List<UserShortDTO> Users);
}
