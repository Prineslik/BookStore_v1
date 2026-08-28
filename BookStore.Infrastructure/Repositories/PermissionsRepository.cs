using AutoMapper;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Permissions;
using BookStore.Core.Entities;
using BookStore.Core.Enums;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories
{
    public class PermissionsRepository: IPermissionsRepository
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper _mapper;
        public ILogger<PermissionsRepository> _logger { get; }

        public PermissionsRepository(BookStoreDbContext context, IMapper mapper, ILogger<PermissionsRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<PermissionEntity>?> GetAll()
        {
            _logger.LogDebug($"Получение всех Permissions");

            var permissionsModels = await _context.Permissions
                .AsNoTracking()
                .ToListAsync();

            //if (permissionsModels.Count == 0)
            //    return Result<List<PermissionsResponse?>>.Failure(Error.ListIsEmpty("Permissions"));

            var permissionsResponse = _mapper.Map<List<PermissionEntity>?>(permissionsModels);

            _logger.LogDebug($"Все Permissions получены");

            return permissionsResponse;
        }

        public async Task<Guid> Create(PermissionEntity newPermissionsEntity)
        {
            try
            {
                _logger.LogDebug("Создание Permission с параметрами {@Permission}", newPermissionsEntity);

                var newPermissionModel = _mapper.Map<PermissionModel>(newPermissionsEntity);

                newPermissionModel.Id = Guid.NewGuid();

                var resultPermission = await _context.AddAsync(newPermissionModel);
                await _context.SaveChangesAsync();

                _logger.LogDebug("Permission с параметрами {@Permission} cоздана", newPermissionsEntity);

                return newPermissionModel.Id;
                //return resultPermission != null
                //    ? Result<Guid>.Success(newPermissionModel.Id)
                //    : Result<Guid>.Failure(Error.Unexpected("Ошибка при создании разрешения"));
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogWarning("Permission с GUID = {@PermissionId} не создан", newPermissionsEntity.Id);

                throw new DuplicateException($"Permission with GUID {newPermissionsEntity.Id} already exists");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning($"DB exception");

                throw new InfrastructureException("Failed to save Permission. " + ex.Message);
            }
        }

        public async Task<Guid> Update(PermissionEntity permissionsEntity)
        {
            try
            {
                _logger.LogDebug("Изменение Permission с GUID = {@PermissionId}", permissionsEntity.Id);

                var updatedPermission = await _context.Permissions
                    .Where(p => p.Id == permissionsEntity.Id)
                    .ExecuteUpdateAsync(p => p
                    .SetProperty(f => f.Code, permissionsEntity.Code)
                    .SetProperty(f => f.Description, permissionsEntity.Description));

                _logger.LogDebug("Permission с GUID = {@PermissionId} изменен", permissionsEntity.Id);

                return permissionsEntity.Id;
                //var saveResult = await _context.SaveChangesAsync();
                //return updateResult > 0
                //    ? Result<Guid>.Success(permissionsRequest.Id)
                //    : Result<Guid>.Failure(Error.Unexpected($"Ошибка при внесении изменеий в разрешение {permissionsRequest.Code}"));
            }
            catch(DbUpdateException ex)
            {
                throw new InfrastructureException($"Failed to update Permission with GUID = {permissionsEntity.Id}\n{ex.Message}");
            }
            
        }

        public async Task<Guid> Delete(Guid id)
        {
            try
            {
                _logger.LogDebug("Удаление Permission с GUID = {@PermissionId}", id);

                var resultDelete = await _context.Permissions
                    .Where(p => p.Id == id)
                    .ExecuteDeleteAsync();

                if (resultDelete <= 0)
                {
                    _logger.LogWarning("Permission с GUID = {@PermissionId} не удален", id);
                    throw new NotFoundException("Permission", id);
                }

                _logger.LogDebug("Role с GUID = {@PermissionId} удален", id);
                return id;
                //return resultDelete > 0
                //    ? Result<Guid>.Success(id)
                //    : Result<Guid>.Failure(Error.NotFound("Permission", "Id", id));
            }
            catch (DbUpdateException ex)
            {
                throw new InfrastructureException($"Failed to delete Permission with GUID = {id}\n{ex.Message}");
            }

        }

        public async Task<PermissionEntity?> GetById(Guid id)
        {
            _logger.LogDebug("Полученеи Permission с GUID = {@PermissionId}", id);

            var permissionModel = await _context.Permissions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            //.FindAsync(id);

            if (permissionModel == null)
                throw new NotFoundException("Permission", id);

            var permissionEntity = _mapper.Map<PermissionEntity?>(permissionModel);

            _logger.LogDebug($"Permission с GUID = {id} получен");

            return permissionEntity;

            //return permissionModel != null
            //    ? Result<PermissionsResponse?>.Success(_mapper.Map<PermissionsResponse?>(permissionModel))
            //    : Result<PermissionsResponse?>.Failure(Error.NotFound("Permission", "Id", id));
        }

        public async Task<List<PermissionEntity?>> GetByCode(string code)
        {
            _logger.LogDebug("Полученеи Permission с code = {@Code}", code);

            var permissionModels = _context.Permissions
                .AsNoTracking()
                .Where(p => p.Code.ToLower().Contains(code.ToLower()))
                .OrderBy(p => p.Code)
                .ToList();

            //if (permissionModels.Count == 0)
            //    return Result<List<PermissionsResponse?>>.Failure(Error.NotFound("Permissions", "Code", code));

            var permissionEntity = _mapper.Map<List<PermissionEntity?>>(permissionModels);

            _logger.LogDebug("Permission с code = {@Code} получен", code);

            return permissionEntity;
        }

        public async Task<PagedResult<PermissionEntity>> GetPaged(PermissionQueryParameters parameters/*, CancellationToken cancellationToken = default*/)
        {
            _logger.LogDebug("Получение Permissions с параметрами = {@Params}", parameters);

            var query = _context.Permissions.AsQueryable();

            query = ApplyFilter(query, parameters);

            var totalCount = await query.CountAsync(/*cancellationToken*/);

            //if (totalCount == 0)
            //    return Result<PagedResult<PermissionsResponse?>>.Failure(Error.NotFound("Permissions", "SearchTerm", parameters.SearchTerm));

            query = ApplySorting(query, parameters);

            var validPageSize = parameters.GetValidPageSize();
            var validPageNumber = parameters.PageNumber > 0
                ? parameters.PageNumber
                : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(/*cancellationToken*/);

            _logger.LogDebug("Permissions с параметрами получены");

            return new PagedResult<PermissionEntity>
                {
                    Items = _mapper.Map<List<PermissionEntity>?>(items),
                    TotalCount = totalCount,
                    PageNumber = validPageNumber,
                    PageSize = validPageSize
                };
        }

        private IQueryable<PermissionModel> ApplyFilter(IQueryable<PermissionModel> query, PermissionQueryParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.Trim().ToLower();
                query = query.Where(r =>
                    r.Code.ToLower().Contains(searchTerm));
            }

            return query;
        }

        private IQueryable<PermissionModel> ApplySorting(IQueryable<PermissionModel> query, PermissionQueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                return query.OrderBy(p => p.Id);
            }

            return parameters.SortDescending
                ? query.OrderByDescending(r => r.Code)
                : query.OrderBy(r => r.Code);

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
