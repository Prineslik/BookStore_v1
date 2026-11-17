using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions.Commands;
using BookStore.Application.Contracts.Permissions.DTO;
using BookStore.Application.Contracts.Permissions.Queries;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
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
        private readonly IPermissionsReadRepository _permissionsReadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PermissionsService(IPermissionsRepository permissionsRepository, IPermissionsReadRepository permissionsReadRepository,
            IUnitOfWork unitOfWork)
        {
            _permissionsRepository = permissionsRepository;
            _permissionsReadRepository = permissionsReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<PermissionShortDTO>>> GetAllPermissions()
        {
            var permissions = await _permissionsReadRepository.GetAll();
            return Result<List<PermissionShortDTO>>.Success(permissions);
        }

        public async Task<Result<PermissionDetailsDTO>> GetPermissionById(Guid id)
        {
            var permission = await _permissionsReadRepository.GetById(id);

            return Result<PermissionDetailsDTO>.Success(permission);
        }

        public async Task<Result<List<PermissionShortDTO>>> GetPermissionsByCode(string code)
        {
            var permisisons = await _permissionsReadRepository.GetByCode(code);

            return Result<List<PermissionShortDTO>>.Success(permisisons);
        }

        public async Task<Result<Guid>> CreatePermission(PermissionCreateCommand createCommand)
        {
            if (await _permissionsReadRepository.IsExistByCode(createCommand.Code))
                return Result<Guid>.Failure(Error.Duplicate("Code", createCommand.Code));

            var permissionEntity = PermissionEntity.Create(
                createCommand.Code,
                createCommand.Description);

            var newPermisionId = await _permissionsRepository.Create(permissionEntity);

            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(newPermisionId);
        }

        public async Task<Result<Guid>> UpdatePermission(PermissionUpdateCommand updateCommand)
        {
            var permissionEntity = await _permissionsRepository.GetById(updateCommand.Id);

            if (permissionEntity == null)
                return Result<Guid>.Failure(Error.NotFound("Permission", "Id", updateCommand.Id.ToString()));

            permissionEntity.ChangeCode(updateCommand.Code);
            permissionEntity.ChangeDescription(updateCommand.Description);

            var updatedPermssionId = await _permissionsRepository.Update(permissionEntity);

            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(updatedPermssionId);
        }

        public async Task<Result<Guid>> DeletePermission(Guid id)
        {
            var permissionEntity = await _permissionsRepository.GetById(id);

            if (permissionEntity != null)
            {
                await _permissionsRepository.Delete(permissionEntity);
                await _unitOfWork.CommitAsync();
            }
                
            return Result<Guid>.Success(id);
        }

        public async Task<Result<PagedResult<PermissionShortDTO>>> GetPagedPermissions(PermissionQueryParameters parameters)
        {
            var pagedPermissions = await _permissionsReadRepository.GetPaged(parameters);
            return Result<PagedResult<PermissionShortDTO>>.Success(pagedPermissions);
        }
    }
}
