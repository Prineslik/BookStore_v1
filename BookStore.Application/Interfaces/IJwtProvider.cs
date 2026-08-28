using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(UserEntity user, List<RoleEntity?> roles);
    }
}
