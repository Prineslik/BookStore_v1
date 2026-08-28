using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Users
{
    public interface IUsersService
    {
        Task<Guid> CreateUser(UserEntity userEntity);
        Task<string> LoginUser(string email, string password);
        Task<Guid> DeleteUser(Guid id);
        Task<List<UserEntity?>> GetAllUsers();
        Task<UserEntity?> GetUserById(Guid id);
        Task<UserEntity?> GetUsersByEmail(string email);
        Task<PagedResult<UserEntity?>> GetPagedUsersAsync(UserQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> UpdateUser(UserEntity userEntity);
    }
}
