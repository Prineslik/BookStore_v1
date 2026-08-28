using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Permissions;
using BookStore.Application.Interfaces.Roles;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Services
{
    public class PermissionsService : IPermissionsService
    {
        private readonly IPermissionsRepository _permissionsRepository;

        public PermissionsService(IPermissionsRepository permissionsRepository)
        {
            _permissionsRepository = permissionsRepository;
        }

        public async Task<List<PermissionEntity?>> GetAllPermissions()
        {
            return await _permissionsRepository.GetAll();
        }
        public async Task<PermissionEntity?> GetPermissionById(Guid id)
        {
            return await _permissionsRepository.GetById(id);
        }

        public async Task<List<PermissionEntity>> GetPermissionsByCode(string code)
        {
            return await _permissionsRepository.GetByCode(code);
        }

        public async Task<Guid> CreatePermission(PermissionEntity permissionEntity)
        {
            var existingPermission = await _permissionsRepository.GetByCode(permissionEntity.Code);

            if (existingPermission != null)
                throw new DuplicateException($"Permission с Code {permissionEntity.Code} уже существует");

            //var permissionEntity = new PermissionEntity(
            //    Guid.NewGuid(), 
            //    permissionsRequest.Code, 
            //    permissionsRequest.Description, 
            //    permissionsRequest.RoleIds);

            var newPermisionId = await _permissionsRepository.Create(permissionEntity);

            return newPermisionId;
        }

        public async Task<Guid> UpdatePermission(PermissionEntity permissionEntity)
        {
            var existingPermission = await _permissionsRepository.GetById(permissionEntity.Id);

            if (existingPermission == null)
                throw new NotFoundException("Permission", permissionEntity.Id);


            var updatedPermssionId = await _permissionsRepository.Update(permissionEntity);

            return updatedPermssionId;
        }

        public async Task<Guid> DeletePermission(Guid id)
        {
            return await _permissionsRepository.Delete(id);
        }

        public Task<PagedResult<PermissionEntity>> GetPagedPermissions(PermissionQueryParameters parameters)
        {
            return _permissionsRepository.GetPaged(parameters);
        }
    }
}
