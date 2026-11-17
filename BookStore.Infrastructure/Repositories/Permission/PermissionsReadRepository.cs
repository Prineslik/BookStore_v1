using AutoMapper.QueryableExtensions;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions.DTO;
using BookStore.Application.Contracts.Permissions.Queries;
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
    public class PermissionsReadRepository : IPermissionsReadRepository
    {
        private readonly BookStoreDbContext _context;
        public PermissionsReadRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<PermissionShortDTO>> GetAll()
        {
            var permissions = await _context.Permissions
                .AsNoTracking()
                .Select(PermissionPersistanceMapping.ToListItemDTOExpr)
                .ToListAsync();

            return permissions;
        }

        public async Task<List<PermissionShortDTO>> GetByCode(string code)
        {
            var permission = await _context.Permissions
                .AsNoTracking()
                .Where(p => p.Code.ToLower().Contains(code.ToLower()))
                .Select(PermissionPersistanceMapping.ToListItemDTOExpr)
                .ToListAsync();

            return permission;
        }

        public async Task<PermissionDetailsDTO?> GetById(Guid id)
        {
            var permissionResponse = await _context.Permissions
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(PermissionPersistanceMapping.ToDetailsDTOExpr)
                .FirstOrDefaultAsync();

            return permissionResponse;
        }
        public async Task<bool> IsExistById(Guid id)
        {
            return await _context.Permissions.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> IsExistByCode(string code)
        {
            return await _context.Permissions.AnyAsync(p => p.Code.Equals(code));
        }

        public async Task<PagedResult<PermissionShortDTO>> GetPaged(PermissionQueryParameters parameters)
        {
            var query = _context.Permissions
                .AsNoTracking()
                .AsQueryable();

            query = ApplyFilter(query, parameters);

            var totalCount = await query.CountAsync(/*cancellationToken*/);

            query = ApplySorting(query, parameters);

            var validPageSize = parameters.GetValidPageSize();
            var validPageNumber = parameters.PageNumber > 0
                ? parameters.PageNumber
                : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(/*cancellationToken*/);

            return new PagedResult<PermissionShortDTO>
            {
                Items = items.AsQueryable()
                    .Select(PermissionPersistanceMapping.ToListItemDTOExpr)
                    .ToList(),
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
