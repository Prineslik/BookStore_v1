using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Roles;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BookStore.Infrastructure.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper _mapper;
        public ILogger<UsersRepository> _logger { get; }

        public RolesRepository(BookStoreDbContext context, IMapper mapper, ILogger<UsersRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<RoleEntity?>> GetAll()
        {
            _logger.LogDebug($"Получение всех Roles");

            var roleModels = await _context.Roles
                .AsNoTracking()
                .Include(r => r.Permissions)
                .ToListAsync();

            //if (roleModels.Count == 0)
            //    return Result<List<RoleEntity?>>.Failure(Error.ListIsEmpty("Roles"));

            var roleEntities = _mapper.Map<List<RoleEntity>>(roleModels);

            _logger.LogDebug($"Все Roles получены");

            return roleEntities;
            //return Result<List<RoleEntity?>>.Success(roleEntities);
        }

        public async Task<Guid> Create(RoleEntity newRoleEntity)
        {
            try
            {
                _logger.LogDebug("Создание User с параметрами {@Role}", newRoleEntity);

                var newRoleModel = _mapper.Map<RoleModel>(newRoleEntity);

                newRoleModel.Permissions = _context.Permissions
                    .Where(p => newRoleEntity.PermissionIds.Contains(p.Id))
                    .ToList();

                var resultRole = await _context.AddAsync(newRoleModel);
                await _context.SaveChangesAsync();

                _logger.LogDebug("Role с параметрами {@Role} успешно создан", newRoleEntity);

                return newRoleEntity.Id;
                /*return resultRole != null
                    ? Result<Guid>.Success(newRoleModel.Id)
                    : Result<Guid>.Failure(Error.Unexpected(""));*/
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogWarning("Role с GUID = {@UserId} не создан", newRoleEntity.Id);

                throw new DuplicateException($"Role with GUID {newRoleEntity.Id} already exists");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning($"DB exception");

                throw new InfrastructureException("Failed to save role. " + ex.Message);
            }
        }

        public async Task<Guid> Update(RoleEntity roleEntity)
        {
            try
            {
                _logger.LogDebug("Изменение Role с GUID = {@RoleId}", roleEntity.Id);

                var roleModel = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == roleEntity.Id);

                /*if (roleModel == null)
                    Result<Guid>.Failure(Error.NotFound("Role", "Id", roleEntity.Id));*/
                
                var updatedPermissions = _context.Permissions
                    //.AsNoTracking()
                    .Where(p => roleEntity.PermissionIds.Contains(p.Id))
                    .ToList();

                roleModel.Name = roleEntity.Name;
                roleModel.Permissions = updatedPermissions;

                _logger.LogDebug("Role с GUID = {@RoleId} изменена", roleEntity.Id);

                return roleModel.Id;
                /*return await _context.SaveChangesAsync() > 0 
                    ? Result<Guid>.Success(roleModel.Id) 
                    : Result<Guid>.Failure(Error.Unexpected($"Ошибка при внесении изменеий в роль {roleModel.Name}"));*/
            }
            catch (DbUpdateException ex)
            {
                throw new InfrastructureException($"Failed to update Role with GUID = {roleEntity.Id}\n{ex.Message}");
            }
        }

        public async Task<Guid> Delete(Guid id)
        {
            try
            {
                _logger.LogDebug("Удаление Role с GUID = {@id}", id);

                var resultDelete = await _context.Roles
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();

                //if (resultDelete <= 0)
                //{
                //    _logger.LogWarning("Role с GUID = {@RoleId} не удалена", id);
                //    throw new NotFoundException("Role", id);
                //}

                _logger.LogDebug("Role с GUID = {@RoleId} удалена", id);

                return id;
                /*return resultDelete > 0
                    ? Result<Guid>.Success(id)
                    : Result<Guid>.Failure(Error.NotFound("Role", "Id", ""));*/
            }
            catch (DbUpdateException ex)
            {
                throw new InfrastructureException($"Failed to delete user with GUID = {id}\n{ex.Message}");
            }
        }

        public async Task<RoleEntity?> GetById(Guid id)
        {
            _logger.LogDebug("Полученеи Role с GUID = {@RoleId}", id);

            var roleModel = await _context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (roleModel == null)
                throw new NotFoundException("Role", id);

            var roleEntity = _mapper.Map<RoleEntity?>(roleModel);

            _logger.LogDebug($"Role с GUID = {id} получен");

            return roleEntity;
            /*return roleModel != null
                ? Result<RoleEntity?>.Success(_mapper.Map<RoleEntity?>(roleModel))
                : Result<RoleEntity?>.Failure(Error.NotFound("Role", "Id", id));*/
        }
        public async Task<List<RoleEntity?>> GetByUser(Guid id)
        {
            _logger.LogDebug("Полученеи Roles c UserId = {@UserId}", id);

            var roleModels = await _context.Roles
                .Where(r => r.Users.Any(u => u.Id == id))
                .ToListAsync();

            var roleEntities = _mapper.Map<List<RoleEntity?>>(roleModels);

            _logger.LogDebug($"Roles c UserId = {id} получены");

            return roleEntities;
            /*return roleModels != null
                ? Result<List<RoleEntity?>>.Success(_mapper.Map<List<RoleEntity?>>(roleModels))
                : Result<List<RoleEntity?>>.Failure(Error.NotFound("Role", "Id", id));*/
        }

        public async Task<List<RoleEntity?>> GetByName(string name)
        {
            _logger.LogDebug("Полученеи Roles c Name = {@RoleName}", name);

            var roleModels = _context.Roles
                .AsNoTracking()
                .Where(r => r.Name.ToLower().Contains(name.ToLower()))
                .OrderBy(r => r.Name)
                .ToList();

            //if (roleModels.Count == 0)
                //return Result<List<RoleEntity?>>.Failure(Error.NotFound("Role", "Name", name));

            var roleEntities = _mapper.Map<List<RoleEntity?>>(roleModels);

            _logger.LogDebug($"Roles c Roles c Name = {name} получены");

            return roleEntities;
            //return Result<List<RoleEntity?>>.Success(roleEntities);
        }

        public async Task<List<RoleEntity?>> GetByNameStrict(string name)
        {
            _logger.LogDebug("Полученеи Roles c Name = {@RoleName}", name);

            var roleModels = _context.Roles
                .AsNoTracking()
                .Where(r => r.Name.ToLower().Equals(name.ToLower()))
                .OrderBy(r => r.Name)
                .ToList();

            //if (roleModels.Count == 0)
                //return Result<List<RoleEntity?>>.Failure(Error.NotFound("Role", "Name", name));

            var roleEntities = _mapper.Map<List<RoleEntity?>>(roleModels);

            _logger.LogDebug($"Roles c Roles c Name = {name} получены");

            return roleEntities;
        }

        public async Task<List<RoleEntity>?> GetByList(List<Guid?> ids)
        {
            _logger.LogDebug("Полученеи Roles по списку Id");

            var roleModels = _context.Roles
                .AsNoTracking()
                .Where(r => ids.Contains(r.Id))
                .ToList();

            //if (roleModels.Count == 0)
                //return Result<List<RoleEntity?>>.Failure(Error.NotFound("Role", "Id", ""));

            var roleEntities = _mapper.Map<List<RoleEntity?>>(roleModels);

            _logger.LogDebug($"Roles по списку Id получены");

            return roleEntities;
            //return Result<List<RoleEntity?>>.Success(roleEntities);
        }

        //public async Task<bool> IsExist(Guid id)
        //{
        //    return _context.Books.FindAsync(id) != null ? true : false;
        //}

        public async Task<PagedResult<RolesResponse?>> GetPaged(RoleQueryParameters parameters/*, CancellationToken cancellationToken = default*/)
        {
            _logger.LogDebug("Полученеи Roles с параметрами = {@Params}", parameters);

            var query = _context.Roles.AsQueryable();

            query = ApplyFilter(query, parameters);

            var totalCount = await query.CountAsync(/*cancellationToken*/);

            //if (totalCount == 0)
                //return Result<PagedResult<RolesResponse?>>.Failure(Error.NotFound("Role", "SearchTerm", parameters.SearchTerm));

            query = ApplySorting(query, parameters);

            var validPageSize = parameters.GetValidPageSize();
            var validPageNumber = parameters.PageNumber > 0 
                ? parameters.PageNumber 
                : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(/*cancellationToken*/);

            return new PagedResult<RolesResponse?>
                {
                    Items = _mapper.Map<List<RolesResponse?>>(items),
                    TotalCount = totalCount,
                    PageNumber = validPageNumber,
                    PageSize = validPageSize
                };
            //return Result<PagedResult<RolesResponse?>>.Success(
            //    new PagedResult<RolesResponse>
            //    {
            //        Items = _mapper.Map<List<RolesResponse>>(items),
            //        TotalCount = totalCount,
            //        PageNumber = validPageNumber,
            //        PageSize = validPageSize
            //    });
        }

        private IQueryable<RoleModel> ApplyFilter(IQueryable<RoleModel> query, RoleQueryParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.Trim().ToLower();
                query = query.Where(r =>
                    r.Name.ToLower().Contains(searchTerm));
            }

            return query;
        }

        private IQueryable<RoleModel> ApplySorting(IQueryable<RoleModel> query, RoleQueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                return query.OrderBy(p => p.Id);
            }

            return parameters.SortDescending
                ? query.OrderByDescending(r => r.Name)
                : query.OrderBy(r => r.Name);

            //return parameters.SortBy.ToLower() switch
            //{
            //    "title" => parameters.SortDescending
            //    ? query.OrderByDescending(p => p.Title)
            //    : query.OrderBy(p => p.Title),

            //    "price" => parameters.SortDescending
            //    ? query.OrderByDescending(p => p.Price)
            //    : query.OrderBy(p => p.Price),

            //    _ => parameters.SortDescending
            //    ? query.OrderByDescending(p => p.Id)
            //    : query.OrderBy(p => p.Id)
            //};
        }
    }
}
