using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles.DTO;
using BookStore.Application.Contracts.Roles.Queries;
using BookStore.Application.Interfaces.Roles;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories.Role
{
    public class RolesReadRepository : IRolesReadRepository
    {
        private readonly BookStoreDbContext _context;
        public ILogger<RolesReadRepository> _logger { get; }

        public RolesReadRepository(BookStoreDbContext context, ILogger<RolesReadRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<RoleShortDTO>> GetAll()
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .Select(RolePersistanceMapping.ToListItemDTOExpr)
                .ToListAsync();

            return roles;
        }

        public async Task<RoleDetailsDTO> GetById(Guid id)
        {
            var role = await _context.Roles
                .AsNoTracking()
                .Select(RolePersistanceMapping.ToDetailsDTOExpr)
                .FirstOrDefaultAsync(r => r.Id == id);

            return role;
        }

        public async Task<List<RoleShortDTO>> GetByName(string name)
        {
            var roleResponse = await _context.Roles
                .AsNoTracking()
                .Where(r => r.Name.ToLower().Contains(name.ToLower()))
                .Select(RolePersistanceMapping.ToListItemDTOExpr)
                .OrderBy(r => r.Name)
                .ToListAsync();

            return roleResponse;
        }

        public async Task<List<RoleShortDTO>> GetByList(List<Guid> ids)
        {
            _logger.LogDebug("Полученеи Roles по списку Id");

            var roleResponse = await _context.Roles
                .AsNoTracking()
                .Where(r => ids.Contains(r.Id))
                .Select(RolePersistanceMapping.ToListItemDTOExpr)
                .ToListAsync();

            _logger.LogDebug($"Roles по списку Id получены");

            return roleResponse;
        }

        public async Task<List<RoleShortDTO>> GetByUser(Guid id)
        {
            _logger.LogDebug("Полученеи Roles c UserId = {@UserId}", id);

            var roleResponse = await _context.UserRole
                .AsNoTracking()
                .Where(ur => ur.UserId == id)
                .Select(ur => new RoleShortDTO(
                    ur.RoleId,
                    ur.Role.Name))
                .OrderBy(r => r.Name)
                .ToListAsync();

            _logger.LogDebug($"Roles c UserId = {id} получены");

            return roleResponse;
        }

        public async Task<bool> IsExistById(Guid id)
        {
            return await _context.Roles.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> IsExistByName(string name)
        {
            return await _context.Roles.AnyAsync(r => r.Name.Equals(name));
        }

        public async Task<PagedResult<RoleShortDTO>> GetPaged(RoleQueryParameters parameters)
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

            return new PagedResult<RoleShortDTO>
            {
                Items = items.Select(model => new RoleShortDTO(
                    model.Id,
                    model.Name))
                .ToList(),
                TotalCount = totalCount,
                PageNumber = validPageNumber,
                PageSize = validPageSize
            };
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

        public async Task<Guid> GetDefaultRole()
        {
            var roleId = await _context.Roles
                .AsNoTracking()
                .Where(r => r.Name.Equals("Default user"))
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            return roleId;
        }
    }
}
