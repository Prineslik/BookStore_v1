using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles.Commands;
using BookStore.Application.Contracts.Roles.DTO;
using BookStore.Application.Contracts.Roles.Queries;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Roles;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Services
{
    public class RolesService : IRolesService
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly IRolesReadRepository _rolesReadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RolesService(IRolesRepository roleRepository, IRolesReadRepository rolesReadRepository, IUnitOfWork unitOfWork)
        {
            _rolesRepository = roleRepository;
            _rolesReadRepository = rolesReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<RoleShortDTO>>> GetAllRoles()
        {
            var roles = await _rolesReadRepository.GetAll();
            return Result<List<RoleShortDTO>>.Success(roles);
        }
        public async Task<Result<RoleDetailsDTO>> GetRoleById(Guid id)
        {
            var role = await _rolesReadRepository.GetById(id);
            if (role == null)
                return Result<RoleDetailsDTO?>.Failure(Error.NotFound("Role", "Id", id));

            return Result<RoleDetailsDTO>.Success(role);
        }

        public async Task<Result<List<RoleShortDTO>>> GetRolesByName(string name)
        {
            var roles = await _rolesReadRepository.GetByName(name);

            return Result<List<RoleShortDTO>>.Success(roles);
        }

        public async Task<Result<Guid>> CreateRole(RoleCreateCommand createCommand)
        {
            if (await _rolesReadRepository.IsExistByName(createCommand.Name))
                return Result<Guid>.Failure(Error.Duplicate("Name", createCommand.Name));

            var roleEntity = RoleEntity.Create(
                createCommand.Name,
                createCommand.PermissionIds.ToList());

            var newRoleId = await _rolesRepository.Create(roleEntity);

            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(newRoleId);
        }

        public async Task<Result<Guid>> UpdateRole(RoleUpdateCommand updateCommand)
        {
            var roleEntity = await _rolesRepository.GetById(updateCommand.Id);

            if (roleEntity == null)
                return Result<Guid>.Failure(Error.NotFound("Role", "Id", updateCommand.Id.ToString()));

            roleEntity.Rename(updateCommand.Name);
            roleEntity.SynchPermissions(updateCommand.PermissionIds.ToList());

            var updatedRoleId = await _rolesRepository.Update(roleEntity);

            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(updatedRoleId);
        }

        public async Task<Result<Guid>> DeleteRole(Guid id)
        {
            var roleEntity = await _rolesRepository.GetById(id);

            if (roleEntity != null)
            { 
                await _rolesRepository.Delete(roleEntity);

                await _unitOfWork.CommitAsync();
            }

            return Result<Guid>.Success(id);
        }

        public async Task<Result<PagedResult<RoleShortDTO>>> GetPagedRoles(RoleQueryParameters parameters)
        {
            var pagedItems = await _rolesReadRepository.GetPaged(parameters);
            return Result<PagedResult<RoleShortDTO>>.Success(pagedItems);
        }

        public async Task<Result<Guid>> AddPermissions(RoleAddPermissionsCommand addPermissionsCommand/*Guid roleId, List<Guid> permissionIds*/)
        {
            var roleEntity = await _rolesRepository.GetById(addPermissionsCommand.Id);

            if (roleEntity == null)
                return Result<Guid>.Failure(Error.NotFound("User", "Id", addPermissionsCommand.Id.ToString()));

            roleEntity.AddPermissions(addPermissionsCommand.PermissionIds.ToList());

            var updatedRoleId = await _rolesRepository.Update(roleEntity);

            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(updatedRoleId);
        }
    }
}