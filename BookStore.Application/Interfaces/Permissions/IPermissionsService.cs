using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions.Commands;
using BookStore.Application.Contracts.Permissions.DTO;
using BookStore.Application.Contracts.Permissions.Queries;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Permissions
{
    public interface IPermissionsService
    {
        Task<Result<Guid>> CreatePermission(PermissionCreateCommand createCommand);
        Task<Result<Guid>> DeletePermission(Guid id);
        Task<Result<List<PermissionShortDTO>>> GetAllPermissions();
        Task<Result<PermissionDetailsDTO>> GetPermissionById(Guid id);
        Task<Result<List<PermissionShortDTO>>> GetPermissionsByCode(string code);
        Task<Result<PagedResult<PermissionShortDTO>>> GetPagedPermissions(PermissionQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Result<Guid>> UpdatePermission(PermissionUpdateCommand updateCommand);
    }
}