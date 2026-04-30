using BookStore.Application.Contracts;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Users
{
    public interface IUsersRepository
    {
        Task<Guid> Create(UserEntity newUserEntity);
        Task<Guid> Delete(Guid id);
        Task<Result<List<UserEntity>>> GetAll();
        Task<Result<UserEntity?>> GetById(Guid id);
        Task<Result<List<UserEntity?>>> GetByEmail(string email);
        Task<Result<PagedResult<UserEntity>>> GetPagedAsync(UserQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> Update(UserEntity userEntity);
        Task<bool> IsExist(Guid id);
    }
}
