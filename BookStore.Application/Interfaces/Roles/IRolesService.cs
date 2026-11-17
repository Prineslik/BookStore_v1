using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles.Commands;
using BookStore.Application.Contracts.Roles.DTO;
using BookStore.Application.Contracts.Roles.Queries;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Roles
{
    public interface IRolesService
    {
        Task<Result<Guid>> CreateRole(RoleCreateCommand createCommand/*RoleEntity roleEntity*/);
        Task<Result<Guid>> DeleteRole(Guid id);
        Task<Result<Guid>> UpdateRole(RoleUpdateCommand updateCommand/*RoleEntity rolesRequest*/);
        Task<Result<Guid>> AddPermissions(RoleAddPermissionsCommand addPermissionsCommand/*Guid roleId ,List<Guid> permissionIds*/);
        Task<Result<List<RoleShortDTO>>> GetAllRoles();
        Task<Result<RoleDetailsDTO>> GetRoleById(Guid id);
        Task<Result<List<RoleShortDTO>>> GetRolesByName(string Name);
        Task<Result<PagedResult<RoleShortDTO>>> GetPagedRoles(RoleQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
    }
}