using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Permissions
{
    public interface IPermissionsService
    {
        Task<Guid> CreatePermission(/*PermissionsRequest*/PermissionEntity permissionsRequest);
        Task<Guid> DeletePermission(Guid id);
        Task<List<PermissionEntity>> GetAllPermissions();
        Task<PermissionEntity?> GetPermissionById(Guid id);
        Task<List<PermissionEntity>> GetPermissionsByCode(string code);
        Task<PagedResult<PermissionEntity>> GetPagedPermissions(PermissionQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> UpdatePermission(PermissionEntity permissionsRequest);
    }
}