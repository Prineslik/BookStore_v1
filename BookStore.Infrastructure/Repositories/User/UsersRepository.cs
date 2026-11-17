using AutoMapper;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Users;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories.User
{
    public class UsersRepository : IUsersRepository
    {
        private readonly BookStoreDbContext _context;
        public ILogger<UsersRepository> _logger { get; }

        public UsersRepository(BookStoreDbContext context, ILogger<UsersRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Create(UserEntity newUserEntity)
        {
            //try
            //{
            _logger.LogDebug("Создание User с параметрами {@User}", newUserEntity);

            var newUserModel = newUserEntity.ToModel();

            await _context.AddAsync(newUserModel);
                
            _logger.LogDebug("User с параметрами {@User} успешно создан", newUserEntity);

            return newUserEntity.Id;
            //}
            //catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            //{
            //    _logger.LogWarning("User с GUID = {@UserId} не создан", newUserEntity.Id);

            //    throw new DuplicateException($"User with GUID {newUserEntity.Id} already exists");
            //}
            //catch (DbUpdateException ex)
            //{
            //    _logger.LogWarning($"DB exception");

            //    throw new InfrastructureException("Failed to save User. " + ex.Message);
            //}
        }

        public async Task<Guid> Delete(UserEntity userEntity)
        {
            //_logger.LogDebug("Удаление User с GUID = {@id}", id);

            _context.Users.Remove(userEntity.ToModel());

            //_logger.LogDebug("Role с GUID = {@UserId} удален", id);
            return userEntity.Id;
        }

        public async Task<UserEntity?> GetById(Guid id)
        {
            _logger.LogDebug("Полученеи User с GUID = {@UserId}", id);

            var userModel = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            //if (userModel == null)
            //    throw new NotFoundException("User", id);

            var roleIds = await _context.UserRole
                .Where(ur => ur.UserId == userModel.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var ownedBookIds = await _context.UserBook
                .Where(ub => ub.UserId == userModel.Id)
                .Select(ub => ub.BookId)
                .ToListAsync();

            var userEntity = userModel is null 
                ? null
                : UserEntity.Reconstitute(
                    userModel.Id,
                    userModel.UserName,
                    userModel.Email,
                    userModel.PasswordHash,
                    userModel.ProfilePhotoURL,
                    userModel.CreateDate,
                    userModel.UpdateDate,
                    userModel.AuthorId,
                    roleIds,
                    ownedBookIds);

            _logger.LogDebug($"User с GUID = {id} получен");

            return userEntity;
        }

        public async Task<Guid> Update(UserEntity userEntity)
        {
            var userModel = await _context.Users
                .Include(u => u.UserRoleModels)
                .Include(u => u.UserBookModels)
                .FirstOrDefaultAsync(u => u.Id == userEntity.Id);

            if (userModel is null)
                throw new InvalidOperationException(
                    $"Cannot save User {userEntity.Id}: not found in database");

            userModel.UserName = userEntity.UserName;
            userModel.Email = userEntity.Email;
            userModel.PasswordHash = userEntity.PasswordHash;
            userModel.ProfilePhotoURL = userEntity.ProfilePhotoURL;
            userModel.CreateDate = userEntity.CreateDate;
            userModel.UpdateDate = userEntity.UpdateDate;
            userModel.IsAuthor = userEntity.IsAuthor;
            userModel.AuthorId = userEntity.AuthorId;

            SyncRoles(userModel, userEntity.RoleIds);
            SyncBooks(userModel, userEntity.OwnedBookIds);

            return userModel.Id;
            
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    _logger.LogWarning(ex, "Конфликт обновления для User с ID = {UserId}", userEntity.Id);

            //    //var entry = ex.Entries.Single();
            //    //var databaseValues = await entry.GetDatabaseValuesAsync();

            //    //if (databaseValues == null)
            //    //{
            //    //    throw new NotFoundException("User", userEntity.Id);
            //    //}

            //    throw new DbConcurrencyException($"User с ID = {userEntity.Id} был изменен другим пользователем. Обновите данные.");
            //}
            //catch (DbUpdateException ex)
            //{
            //    throw new InfrastructureException($"Failed to update User with GUID = {userEntity.Id}\n{ex.Message}");
            //}
        }

        private void SyncRoles(UserModel user, IReadOnlyCollection<Guid> roleIds)
        {
            var target = roleIds.ToHashSet();
            user.UserRoleModels.RemoveAll(ur => !target.Contains(ur.RoleId));
            var current = user.UserRoleModels.Select(ur => ur.RoleId).ToHashSet();
            foreach (var roleId in target.Except(current))
                user.UserRoleModels.Add(new UserRoleModel 
                {
                    UserId = user.Id,
                    RoleId = roleId
                });
        }

        private void SyncBooks(UserModel user, IReadOnlyCollection<Guid> bookIds)
        {
            var target = bookIds.ToHashSet();
            user.UserBookModels.RemoveAll(ub => !bookIds.Contains(ub.BookId));
            var current = user.UserBookModels.Select(ub => ub.BookId).ToHashSet();
            foreach (var bookId in target.Except(current))
                user.UserBookModels.Add(new UserBookModel 
                {
                    UserId = user.Id,
                    BookId = bookId
                });
        }
    }
}
