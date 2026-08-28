using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Permissions
{
    public interface IPermissionsRepository
    {
        Task<Guid> Create(PermissionEntity permissionEntity);
        Task<Guid> Delete(Guid id);
        Task<List<PermissionEntity>?> GetAll();
        Task<PermissionEntity?> GetById(Guid id);
        Task<List<PermissionEntity?>> GetByCode(string code);
        //Task<bool> IsExist(Guid id);
        Task<PagedResult<PermissionEntity>> GetPaged(PermissionQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> Update(PermissionEntity permissionEntity);
    }
}
