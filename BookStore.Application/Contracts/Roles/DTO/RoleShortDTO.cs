using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Roles.DTO
{
    public record RoleShortDTO(
        Guid Id,
        string Name);
}
