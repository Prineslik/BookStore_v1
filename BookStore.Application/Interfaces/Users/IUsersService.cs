using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users.Commands;
using BookStore.Application.Contracts.Users.DTO;
using BookStore.Application.Contracts.Users.Queries;
using BookStore.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Users
{
    public interface IUsersService
    {
        Task<Result<Guid>> CreateUser(RegisterUserCommand registerUserCommand);
        Task<Result<string>> LoginUser(string email, string password);
        Task<Result<Guid>> DeleteUser(Guid id);
        Task<Result<Guid>> UpdateUserProfile(UpdateUserProfileCommand updateUserProfileCommand);
        Task<Result<List<UserShortDTO?>>> GetAllUsers();
        Task<Result<UserDetailsDTO?>> GetUserById(Guid id);
        Task<Result<UserAuthDTO?>> GetUserByEmail(string email);
        Task<Result<PagedResult<UserShortDTO?>>> GetPagedUsersAsync(UserQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
    }
}
