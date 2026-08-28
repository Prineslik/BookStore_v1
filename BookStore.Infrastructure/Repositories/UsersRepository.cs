using AutoMapper;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Users;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private const string DEFAULT_PHOTO_URL = "";

        private readonly BookStoreDbContext _context;
        private readonly IMapper _mapper;
        public ILogger<UsersRepository> _logger { get; }
        public UsersRepository(BookStoreDbContext context, IMapper mapper, ILogger<UsersRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Guid> Create(UserEntity newUserEntity/*, List<RoleEntity?> roles*/)
        {
            try
            {
                _logger.LogDebug("Создание User с параметрами {@User}", newUserEntity);

                var newUserModel = _mapper.Map<UserModel>(newUserEntity);//new UserModel(userEntity.Id, userEntity.Name, userEntity.Email, userEntity.PasswordHash, userEntity.ProfilePhotoURL);

                newUserModel.Roles = _context.Roles
                    //.AsNoTracking()
                    .Where(r => newUserEntity.RoleIds.Contains(r.Id))
                    .ToList();
                //_mapper.Map<List<RoleModel?>>(roles);

                //while (await IsExist(newUserModel.Id))
                //    newUserModel.Id = Guid.NewGuid();

                var addUser = await _context.AddAsync(newUserModel);

                await _context.SaveChangesAsync();

                _logger.LogDebug("User с параметрами {@User} успешно создан", newUserEntity);

                return newUserEntity.Id;
                //return addUser != null ? Result<Guid>.Success(newUserModel.Id) : Result<Guid>.Failure(Error.Unexpected($"Ошибка при записи нового пользователя {newUserEntity.Name}"));//newUserModel.Id;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogWarning("User с GUID = {@UserId} не создан", newUserEntity.Id);

                throw new DuplicateException($"User with GUID {newUserEntity.Id} already exists");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning($"DB exception");

                throw new InfrastructureException("Failed to save User. " + ex.Message);
            }
        }

        public async Task<Guid> Delete(Guid id)
        {
            try
            {
                _logger.LogDebug("Удаление User с GUID = {@id}", id);

                var resultDelete = await _context.Users
                    .Where(u => u.Id == id)
                    .ExecuteDeleteAsync();

                if (resultDelete <= 0)
                {
                    _logger.LogWarning("User с GUID = {@UserId} не удален", id);
                    throw new NotFoundException("User", id);
                }

                _logger.LogDebug("Role с GUID = {@UserId} удален", id);
                return id;

            }
            catch (DbUpdateException ex)
            {
                throw new InfrastructureException($"Failed to delete user with GUID = {id}\n{ex.Message}");
            }
            /*return resultDelete > 0
                ? Result<Guid>.Success(id) 
                : Result<Guid>.Failure(Error.Unexpected("Ошибка при удалении пользователя"));*/
        }

        public async Task<List<UserEntity>> GetAll()
        {
            _logger.LogDebug($"Получение всех Users");

            var userModels = await _context.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .ToListAsync();

            //if (userModels.Count == 0)
            //    Result<List<UserEntity>>.Failure(Error.ListIsEmpty("Books"));

            var userEntities = _mapper.Map<List<UserEntity>>(userModels);

            _logger.LogDebug($"Все Users получены");

            return userEntities;
        }

        public async Task<UserEntity?> GetByEmail(string email)
        {
            _logger.LogDebug("Полученеи User с email = {@Email}", email);

            var userModel = await _context.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(b => b.Email.Equals(email));
                //.Where(b => b.Email.ToUpper().Equals(email.ToUpper()))
                //.OrderBy(b => b.UserName)
                //.ToList();

            //if (userModel == null)
            //    return Result<UserEntity?>.Failure(Error.NotFound("User", "Email", email));

            var userEntity = _mapper.Map<UserEntity?>(userModel);

            _logger.LogDebug("User с email = {@Email} получен", email);

            return userEntity;
        }

        public async Task<UserEntity?> GetById(Guid id)
        {
            _logger.LogDebug("Полученеи User с GUID = {@UserId}", id);

            var userModel = await _context.Users
                .AsNoTracking()
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);
            //.FindAsync(id);

            if (userModel == null)
                throw new NotFoundException("User", id);
                //return Result<UserEntity?>.Failure(Error.NotFound("User", "Id", id));

            var userEntity = _mapper.Map<UserEntity?>(userModel);

            _logger.LogDebug($"User с GUID = {id} получен");

            return userEntity;
        }

        public async Task<Guid> Update(UserEntity userEntity)
        {
            try
            {
                _logger.LogDebug("Изменение User с GUID = {@UserId}", userEntity.Id);

                var userModel = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userEntity.Id);

                var roles = _context.Roles
                    .Where(r => userEntity.RoleIds.Contains(r.Id))
                    .ToList();

                userModel.UserName = userEntity.Name;
                userModel.ProfilePhotoURL = userEntity.ProfilePhotoURL;
                userModel.Email = userEntity.Email;
                userModel.PasswordHash = userEntity.PasswordHash;

                userModel.Roles.Clear();

                if (roles != null)
                    userModel.Roles.AddRange(roles);

                /*var affectedRows = */
                await _context.SaveChangesAsync();

                _logger.LogDebug("User с GUID = {@UserId} изменена", userEntity.Id);

                return userModel.Id;
                /*return affectedRows > 0
                    ? Result<Guid>.Success(userEntity.Id)
                    : Result<Guid>.Failure(Error.Unexpected($"Ошибка при внесении изменеий в пользователя {userEntity.Name}"));*/
            }
            catch (DbUpdateException ex)
            {
                throw new InfrastructureException($"Failed to update User with GUID = {userEntity.Id}\n{ex.Message}");
            }
        }

        public async Task<bool> IsExist(Guid id)
        {
            _logger.LogDebug("Проверка на наличие User");

            return await _context.Users.FindAsync(id) != null ? true : false;

            /*return Result<bool>.Success(await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id) != null 
                ? true 
                : false);*/
        }

        public async Task<PagedResult<UserEntity?>> GetPagedAsync(UserQueryParameters parameters/*, CancellationToken cancellationToken = default*/)
        {
            _logger.LogDebug("Полученеи User с параметрами = {@Params}", parameters);

            var query = _context.Users.AsNoTracking().AsQueryable();

            query = ApplyFilter(query, parameters);

            var totalCount = await query.CountAsync(/*cancellationToken*/);

            /*if (totalCount == 0)
                return Result<PagedResult<UserEntity?>>.Failure(Error.NotFound("User", "", parameters.SearchTerm));*/

            query = ApplySorting(query, parameters);

            var validPageSize = parameters.GetValidPageSize();
            var validPageNumber = parameters.PageNumber > 0 ? parameters.PageNumber : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(/*cancellationToken*/);

            _logger.LogDebug("Users с параметрами получены");

            return new PagedResult<UserEntity?>
            {
                Items = _mapper.Map<List<UserEntity?>>(items),
                TotalCount = totalCount,
                PageNumber = validPageNumber,
                PageSize = validPageSize
            };
            /*return Result<PagedResult<UserEntity?>>.Success(
                new PagedResult<UserEntity?>
                {
                    Items = _mapper.Map<List<UserEntity?>>(items),
                    TotalCount = totalCount,
                    PageNumber = validPageNumber,
                    PageSize = validPageSize
                });*/
        }

        private IQueryable<UserModel> ApplyFilter(IQueryable<UserModel> query, UserQueryParameters parameters)
        {
            _logger.LogDebug("Применение параметров фильтрации поиска Users");
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.UserName.ToLower().Contains(searchTerm) ||
                    p.Email.ToLower().Contains(searchTerm));
            }
            _logger.LogDebug("Параметры фильтрации поиска Users успешно применены");

            //if (parameters.CategoryId.HasValue)
            //{
            //    query = query.Where(p => p.CategoryId == parameters.CategoryId.Value);
            //}

            return query;
        }

        private IQueryable<UserModel> ApplySorting(IQueryable<UserModel> query, UserQueryParameters parameters)
        {
            _logger.LogDebug("Применение параметров сортировки поиска Users");

            if (string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                return query.OrderBy(p => p.Id);
            }

            _logger.LogDebug("Параметры сортировки поиска Users успешно применены");

            return parameters.SortBy.ToLower() switch
            {
                "name" => parameters.SortDescending
                ? query.OrderByDescending(p => p.UserName)
                : query.OrderBy(p => p.UserName),

                "email" => parameters.SortDescending
                ? query.OrderByDescending(p => p.Email)
                : query.OrderBy(p => p.Email),

                "email_confirmed" => parameters.SortDescending
                ? query.OrderByDescending(p => p.EmailConfirmed)
                : query.OrderBy(p => p.EmailConfirmed),

                _ => parameters.SortDescending
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id)
            };
        }
    }
}
