using BookStore.Application.Contracts.Roles.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Users.DTO
{
    public record UserAuthDTO(
        Guid Id,
        string Email,
        string PasswordHash,
        List<RoleShortDTO> Roles
    );
}
