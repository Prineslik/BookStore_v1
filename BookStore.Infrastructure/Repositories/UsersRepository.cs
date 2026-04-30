using AutoMapper;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Interfaces.Users;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
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
        public UsersRepository(BookStoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid> Create(UserEntity newUserEntity)
        {
            var newUserModel = _mapper.Map<UserModel>(newUserEntity);//new UserModel(userEntity.Id, userEntity.Name, userEntity.Email, userEntity.PasswordHash, userEntity.ProfilePhotoURL);

            //while (await IsExist(newUserModel.Id))
            //    newUserModel.Id = Guid.NewGuid();

            await _context.AddAsync(newUserModel);
            await _context.SaveChangesAsync();

            return newUserModel.Id;
            
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        public async Task<Result<List<UserEntity>>> GetAll()
        {
            var userModels = await _context.Users.AsNoTracking().ToListAsync();

            if (userModels.Count == 0)
                Result<List<UserEntity>>.Failure(Error.ListIsEmpty("users"));

            var userEntities = _mapper.Map<List<UserEntity>>(userModels);

            return Result<List<UserEntity>>.Success(userEntities);
        }

        public async Task<Result<List<UserEntity?>>> GetByEmail(string email)
        {
            var userModels = _context.Users
               .Where(b => b.NormalizedEmail.Contains(email.ToUpper()))
               .OrderBy(b => b.UserName)
               .ToList();

            if (userModels == null)
                return Result<List<UserEntity?>>.Failure(Error.NotFound("User", "Email", email));

            var userEntities = _mapper.Map<List<UserEntity?>>(userModels);//entityBook.Select(b => BookEntity.Create(b.Id, b.Title, b.Description, b.Price).Book).ToList();//Book.Create(entityBook.Id, entityBook.Title, entityBook.Description, entityBook.Price).Book;

            return Result<List<UserEntity?>>.Success(userEntities);
        }

        public async Task<Result<UserEntity?>> GetById(Guid id)
        {
            var userModel = await _context.Users
                .FindAsync(id);

            if (userModel == null)
                return Result<UserEntity?>.Failure(Error.NotFound("User", "Id", id));

            var userEntity = _mapper.Map<UserEntity?>(userModel);

            return Result<UserEntity?>.Success(userEntity);
        }

        public async Task<Guid> Update(UserEntity userEntity)
        {
            await _context.Users
                .Where(b => b.Id == userEntity.Id)
                .ExecuteUpdateAsync(u => u
                .SetProperty(f => f.UserName, userEntity.Name)
                .SetProperty(f => f.ProfilePhotoURL, String.IsNullOrWhiteSpace(userEntity.ProfilePhotoURL) ? userEntity.ProfilePhotoURL : DEFAULT_PHOTO_URL)
                .SetProperty(f => f.Email, userEntity.Email));

            return userEntity.Id;
        }

        public async Task<bool> IsExist(Guid id)
        {
            return await _context.Users.FindAsync(id) != null ? true : false;
        }

        public async Task<PagedResult<UserEntity?>> GetPagedAsync(UserQueryParameters parameters/*, CancellationToken cancellationToken = default*/)
        {
            var query = _context.Users.AsQueryable();

            query = ApplyFilter(query, parameters);

            var totalCount = await query.CountAsync(/*cancellationToken*/);

            if (totalCount == 0)
                return Result<PagedResult<UserEntity?>>.Failure(Error.NotFound("User", ""));

            query = ApplySorting(query, parameters);

            

            var validPageSize = parameters.GetValidPageSize();
            var validPageNumber = parameters.PageNumber > 0 ? parameters.PageNumber : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(/*cancellationToken*/);

            return new PagedResult<UserEntity>
            {
                Items = _mapper.Map<List<UserEntity>>(items),
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
    }
}
