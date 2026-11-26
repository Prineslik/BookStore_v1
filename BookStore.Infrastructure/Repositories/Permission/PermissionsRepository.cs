using AutoMapper;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Permissions;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories.Permission
{
    public class PermissionsRepository: IPermissionsRepository
    {
        private readonly BookStoreDbContext _context;
        public ILogger<PermissionsRepository> _logger { get; }

        public PermissionsRepository(BookStoreDbContext context, ILogger<PermissionsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Create(PermissionEntity newPermissionsEntity)
        {
            //try
            //{
                //_logger.LogDebug("Создание Permission с параметрами {@Permission}", newPermissionsEntity);

                var newPermissionModel = newPermissionsEntity.ToModel();

                await _context.AddAsync(newPermissionModel);

                //_logger.LogDebug("Permission с параметрами {@Permission} cоздана", newPermissionsEntity);

                return newPermissionModel.Id;
            //}
            //catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            //{
            //    //_logger.LogWarning("Permission с GUID = {@PermissionId} не создан", newPermissionsEntity.Id);

            //    throw new DuplicateException($"Permission with GUID {newPermissionsEntity.Id} already exists");
            //}
            //catch (DbUpdateException ex)
            //{
            //    _logger.LogWarning($"DB exception");

            //    throw new InfrastructureException("Failed to save Permission. " + ex.Message);
            //}
        }

        public async Task<Guid> Update(PermissionEntity permissionsEntity)
        {
            var permissionModel = await _context.Permissions
                .Include(p => p.PermissionRoleModels)
                .FirstOrDefaultAsync(p => p.Id == permissionsEntity.Id);

            if (permissionModel is null)
                throw new InvalidOperationException(
                    $"Cannot update Permission {permissionModel.Id}: not found in database");

            permissionModel.Code = permissionsEntity.Code;
            permissionModel.Description = permissionsEntity.Description;

            //SyncRoles(permissionModel, permissionsEntity.RoleIds);

            return permissionsEntity.Id;

            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    _logger.LogWarning(ex, "Конфликт обновления для Permission с ID = {PermissionId}", permissionsEntity.Id);

            //    var entry = ex.Entries.Single();
            //    var databaseValues = await entry.GetDatabaseValuesAsync();

            //    if (databaseValues == null)
            //    {
            //        throw new NotFoundException("Permission", permissionsEntity.Id);
            //    }

            //    throw new DbConcurrencyException($"Permission с ID = {permissionsEntity.Id} было изменено другим пользователем. Обновите данные.");
            //}
            //catch (DbUpdateException ex)
            //{
            //    throw new InfrastructureException($"Failed to update Permission with GUID = {permissionsEntity.Id}\n{ex.Message}");
            //}
        }

        //private void SyncRoles(PermissionModel permission, IReadOnlyCollection<Guid> roleIds)
        //{
        //    var target = roleIds.ToHashSet();
        //    permission.PermissionRoleModels.RemoveAll(pr => !target.Contains(pr.RoleId));
        //    var current = permission.PermissionRoleModels.Select(ur => ur.RoleId).ToHashSet();
        //    foreach (var roleId in target.Except(current))
        //        permission.PermissionRoleModels.Add(new PermissionRoleModel
        //        {
        //            PermissionId = permission.Id,
        //            RoleId = roleId
        //        });
        //}

        public async Task<Guid> Delete(PermissionEntity permission)
        {
            //try
            //{
            //    _logger.LogDebug("Удаление Permission с GUID = {@PermissionId}", id);

            _context.Permissions.Remove(permission.ToModel());
                
            //_logger.LogDebug("Role с GUID = {@PermissionId} удален", id);
            return permission.Id;
            //}
            //catch (DbUpdateException ex)
            //{
            //    throw new InfrastructureException($"Failed to delete Permission with GUID = {id}\n{ex.Message}");
            //}
        }

        public async Task<PermissionEntity?> GetById(Guid id)
        {
            //_logger.LogDebug("Полученеи Permission с GUID = {@PermissionId}", id);

            var permissionModel = await _context.Permissions
                .AsNoTracking()
                .Include(p => p.PermissionRoleModels)
                .FirstOrDefaultAsync(p => p.Id == id);

            //_logger.LogDebug($"Permission с GUID = {id} получен");

            return permissionModel.ToDomain();
        }

    }
}
