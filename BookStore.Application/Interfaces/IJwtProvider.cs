using BookStore.Application.Contracts.Roles.DTO;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(Guid userId, List<RoleShortDTO> roles);
    }
}
