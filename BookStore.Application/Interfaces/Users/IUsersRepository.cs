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
        Task<Guid> Create(UserEntity newUserEntity/*, List<RoleEntity?> roles*/);
        Task<Guid> Delete(Guid id);
        Task<List<UserEntity>> GetAll();
        Task<UserEntity?> GetById(Guid id);
        Task<UserEntity?> GetByEmail(string email);
        Task<PagedResult<UserEntity>> GetPagedAsync(UserQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> Update(UserEntity userEntity);
        Task<bool> IsExist(Guid id);
    }
}