using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Roles;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Models;
using BookStore.Infrastructure.Repositories.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BookStore.Infrastructure.Repositories.Role
{
    public class RolesRepository : IRolesRepository
    {
        private readonly BookStoreDbContext _context;
        public ILogger<UsersRepository> _logger { get; }

        public RolesRepository(BookStoreDbContext context, ILogger<UsersRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Create(RoleEntity newRoleEntity)
        {
            //try
            //{
                //_logger.LogDebug("Создание User с параметрами {@Role}", newRoleEntity);

            var newRoleModel = newRoleEntity.ToModel();

            //newRoleModel.Permissions = _context.Permissions
            //    .Where(p => newRoleEntity.PermissionIds.Contains(p.Id))
            //    .ToList();

            await _context.AddAsync(newRoleModel);
                
            //_logger.LogDebug("Role с параметрами {@Role} успешно создан", newRoleModel);

            return newRoleModel.Id;
            //}
            //catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            //{
            //    //_logger.LogWarning("Role с GUID = {@RoleId} уже существует", newRoleEntity.Id);

            //    throw new DuplicateException($"Role with GUID {newRoleEntity.Id} already exists");
            //}
            //catch (DbUpdateException ex)
            //{
            //    _logger.LogWarning($"DB exception");

            //    throw new InfrastructureException("Failed to save role. " + ex.Message);
            //}
        }

        public async Task<Guid> Update(RoleEntity roleEntity)
        {
            //_logger.LogDebug("Изменение Role с GUID = {@RoleId}", roleEntity.Id);

            var roleModel = await _context.Roles
                .Include(r => r.UserRoleModels)
                //.Include(r => r.PermissionRoleModels)
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.Id == roleEntity.Id);

            if (roleModel is null)
                throw new InvalidOperationException(
                    $"Cannot update Role {roleEntity.Id}: not found in database");

            roleModel.Name = roleEntity.Name;

            SyncPermissions(roleModel, roleEntity.PermissionIds);
            //SyncUsers(roleModel, roleEntity.UserIds);
            
            //_logger.LogDebug("Role с GUID = {@RoleId} изменена", roleModel.Id);

            return roleModel.Id;
            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    _logger.LogWarning(ex, "Конфликт обновления для Role с ID = {RoleId}", roleEntity.Id);

            //    var entry = ex.Entries.Single();
            //    var databaseValues = await entry.GetDatabaseValuesAsync();

            //    if (databaseValues == null)
            //    {
            //        throw new NotFoundException("Role", roleEntity.Id);
            //    }

            //    throw new DbConcurrencyException($"Роль с ID = {roleEntity.Id} была изменена другим пользователем. Обновите данные.");
            //}
            //catch (DbUpdateException ex)
            //{
            //    throw new InfrastructureException($"Ошибка изменения Role с GUID = {roleEntity.Id}\n{ex.Message}");
            //}
        }

        private void SyncPermissions(RoleModel role, IReadOnlyCollection<Guid> permissionsIds)
        {
            var target = permissionsIds.ToHashSet();
            role.PermissionRoleModels.RemoveAll(pr => !target.Contains(pr.PermissionId));
            var current = role.PermissionRoleModels.Select(pr => pr.PermissionId).ToHashSet();
            foreach (var permissionId in target.Except(current))
                role.PermissionRoleModels.Add(new PermissionRoleModel
                {
                    PermissionId = permissionId,
                    RoleId = role.Id
                });
        }

        //private void SyncUsers(RoleModel role, IReadOnlyCollection<Guid> usersIds)
        //{
        //    var target = usersIds.ToHashSet();
        //    role.UserRoleModels.RemoveAll(ur => !target.Contains(ur.UserId));
        //    var current = role.UserRoleModels.Select(ur => ur.UserId).ToHashSet();
        //    foreach (var userId in target.Except(current))
        //        role.UserRoleModels.Add(new UserRoleModel
        //        {
        //            UserId = userId,
        //            RoleId = role.Id
        //        });
        //}

        public async Task<Guid> Delete(RoleEntity roleEntity)
        {
            //try
            //{
            //_logger.LogDebug("Удаление Role с GUID = {@id}", roleEntity.Id);

            _context.Roles.Remove(roleEntity.ToModel());
                //.Where(b => b.Id == id)
                //.ExecuteDeleteAsync();

            //_logger.LogDebug("Role с GUID = {@RoleId} удалена", roleEntity.Id);

            return roleEntity.Id;
            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    throw new InfrastructureException($"User with GUID = {id} was modified by another user");
            //}
        }

        public async Task<RoleEntity> GetById(Guid id)
        {
            //_logger.LogDebug("Полученеи Role с GUID = {@RoleId}", id);

            var roleModel = await _context.Roles
                .AsNoTracking()
                .Include(r => r.UserRoleModels)
                .Include(r => r.PermissionRoleModels)
                .FirstOrDefaultAsync(r => r.Id == id);

            //_logger.LogDebug($"Role с GUID = {id} получен");

            return roleModel.ToDomain();
        }
    }
}
