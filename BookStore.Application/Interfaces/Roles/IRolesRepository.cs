using BookStore.Application.Contracts.Roles;
using BookStore.Application.Contracts.Common;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Roles
{
    public interface IRolesRepository
    {
        Task<Guid> Create(RoleEntity roleEntity);
        Task<Guid> Delete(RoleEntity roleEntity);
        Task<Guid> Update(RoleEntity updatedRoleEntity);
        Task<RoleEntity?> GetById(Guid id);
    }
}
