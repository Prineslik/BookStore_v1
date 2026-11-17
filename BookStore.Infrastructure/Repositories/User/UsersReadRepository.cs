using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users.DTO;
using BookStore.Application.Contracts.Users.Queries;
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
    public class UsersReadRepository : IUsersReadRepository
    {
        private readonly BookStoreDbContext _context;

        public UsersReadRepository(BookStoreDbContext context/*, ILogger<UserReadRepository> logger*/)
        {
            _context = context;
        }
        public async Task<List<UserShortDTO>> GetAll()
        {
            var userDTOs = await _context.Users
                .AsNoTracking()
                .Select(UserPersistanceMapping.ToListItemDTOExpr)
                .ToListAsync();

            return userDTOs;
        }

        public async Task<UserAuthDTO?> GetByEmail(string email)
        {
            var userDTO = await _context.Users
                .AsNoTracking()
                .Where(u => u.Email == email)
                .Select(UserPersistanceMapping.ToAuthDTOExpr)
                .FirstOrDefaultAsync();

            return userDTO;
        }

        public async Task<UserDetailsDTO?> GetById(Guid id)
        {
            var userDTO = await _context.Users
                .AsNoTracking()
                .Select(UserPersistanceMapping.ToDetailsDTOExpr)
                .FirstOrDefaultAsync(u => u.Id == id);

            return userDTO;
        }
        public async Task<bool> IsExistById(Guid id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> IsExistByEmail(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<PagedResult<UserShortDTO>> GetPagedAsync(UserQueryParameters parameters)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            query = ApplyFilter(query, parameters);

            var totalCount = await query.CountAsync(/*cancellationToken*/);

            query = ApplySorting(query, parameters);

            var validPageSize = parameters.GetValidPageSize();
            var validPageNumber = parameters.PageNumber > 0 ? parameters.PageNumber : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(/*cancellationToken*/);

            return new PagedResult<UserShortDTO>
            {
                Items = items.AsQueryable()
                .Select(UserPersistanceMapping.ToListItemDTOExpr)
                .ToList(),
                TotalCount = totalCount,
                PageNumber = validPageNumber,
                PageSize = validPageSize
            };
        }

        private IQueryable<UserModel> ApplyFilter(IQueryable<UserModel> query, UserQueryParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.UserName.ToLower().Contains(searchTerm) ||
                    p.Email.ToLower().Contains(searchTerm));
            }

            //if (parameters.CategoryId.HasValue)
            //{
            //    query = query.Where(p => p.CategoryId == parameters.CategoryId.Value);
            //}

            return query;
        }

        private IQueryable<UserModel> ApplySorting(IQueryable<UserModel> query, UserQueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                return query.OrderBy(p => p.Id);
            }

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

        public async Task<bool> HasPermission(Guid userId, string permissionCode)
        {
            var userPermissionCodes = (await _context.UserRole
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.PermissionRoleModels)
                .Select(pr => pr.Permission.Code)
                .ToListAsync())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return userPermissionCodes.Contains(permissionCode);
        }
    }
}
