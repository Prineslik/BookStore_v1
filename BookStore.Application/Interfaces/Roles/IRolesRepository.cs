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
        Task<Guid> Delete(Guid id);
        Task<List<RoleEntity?>> GetAll();
        Task<RoleEntity?> GetById(Guid id);
        Task<List<RoleEntity?>> GetByName(string name);
        Task<List<RoleEntity?>> GetByNameStrict(string name);
        Task<List<RoleEntity?>> GetByList(List<Guid?> ids);
        Task<List<RoleEntity?>> GetByUser(Guid ids);
        //Task<bool> IsExist(Guid id);
        Task<PagedResult<RolesResponse?>> GetPaged(RoleQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> Update(RoleEntity updatedRoleEntity);
    }
}
