using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Roles;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Services
{
    public class RolesService : IRolesService
    {
        private readonly IRolesRepository _roleRepository;

        public RolesService(IRolesRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleEntity>> GetAllRoles()
        {
            return await _roleRepository.GetAll();
        }
        public async Task<RoleEntity?> GetRoleById(Guid id)
        {
            return await _roleRepository.GetById(id);
        }

        public async Task<List<RoleEntity>> GetRolesByName(string name)
        {
            return await _roleRepository.GetByName(name);
        }

        public async Task<Guid> CreateRole(RolesRequest rolesRequest)
        {
            var existingRole = await _roleRepository.GetByNameStrict(rolesRequest.Name);

            if (existingRole != null)
                throw new DuplicateException($"Role с Name = {rolesRequest.Name} уже существует");
            //return Result<Guid>.Failure(Error.Duplicate("Name", rolesRequest.Name));

            var (roleEntity, error) = RoleEntity.Create(
                Guid.NewGuid(),
                rolesRequest.Name,
                rolesRequest.PermissionIds);

            if (!string.IsNullOrEmpty(error))
                throw new ValidationException(error);

            var newRoleId = await _roleRepository.Create(roleEntity);
            return newRoleId;
           /*return string.IsNullOrEmpty(error)
                ? await _roleRepository.Create(roleEntity)
                : Result<Guid>.Failure(Error.Validation(error));*/
        }

        public async Task<Guid> UpdateRole(RolesRequest rolesRequest)
        {
            var existingRoleEntity = await _roleRepository.GetById(rolesRequest.Id);

            if (existingRoleEntity == null)
                throw new NotFoundException("Role", rolesRequest.Id);

            var (roleEntity, error) = RoleEntity.Create(
                rolesRequest.Id,
                rolesRequest.Name,
                rolesRequest.PermissionIds);

            if (!string.IsNullOrEmpty(error))
                throw new ValidationException(error);

            var updatedRoleId = await _roleRepository.Update(roleEntity);

            return updatedRoleId;
            /*return string.IsNullOrEmpty(error)
                ? await _roleRepository.Update(roleEntity)
                : Result<Guid>.Failure(Error.Validation(error));*/
        }

        public async Task<Guid> DeleteRole(Guid id)
        {
            var existingRoleEntity = await _roleRepository.GetById(id);

            if (existingRoleEntity == null)
                throw new NotFoundException("Role", id);

            return await _roleRepository.Delete(id);
        }

        public Task<PagedResult<RolesResponse>> GetPagedRoles(RoleQueryParameters parameters)
        {
            return _roleRepository.GetPaged(parameters);
        }

        public async Task<Guid> AddPermissions(Guid roleId, HashSet<Guid?> permissionIds)
        {
            var existingRoleEntity = await _roleRepository.GetById(roleId);

            if (existingRoleEntity == null)
                throw new NotFoundException("Role", roleId);

            existingRoleEntity.AddPermissions(permissionIds);

            return await _roleRepository.Update(existingRoleEntity);
        }
    }
}