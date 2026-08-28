using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Roles
{
    public interface IRolesService
    {
        Task<Guid> CreateRole(RoleEntity roleEntity);
        Task<Guid> DeleteRole(Guid id);
        Task<List<RoleEntity>> GetAllRoles();
        Task<RoleEntity?> GetRoleById(Guid id);
        Task<List<RoleEntity>> GetRolesByName(string Name);
        Task<PagedResult<RolesResponse>> GetPagedRoles(RoleQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> UpdateRole(RoleEntity rolesRequest/*BookEntity Role*//*Guid id, string title, string description, decimal price*/);
        Task<Guid> AddPermissions(Guid roleId ,HashSet<Guid?> permissionIds);
    }
}