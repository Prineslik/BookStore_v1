using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Users
{
    public interface IUsersService
    {
        Task<Result<Guid>> CreateUser(UsersRequest userEntity);
        Task<Guid> DeleteUser(Guid id);
        Task<Result<List<UserEntity>>> GetAllUsers();
        Task<Result<UserEntity?>> GetUserById(Guid id);
        Task<Result<List<UserEntity?>>> GetUsersByEmail(string email);
        Task<PagedResult<UserEntity>> GetPagedBookAsync(UserQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> UpdateUser(UserEntity userEntity);
    }
}
