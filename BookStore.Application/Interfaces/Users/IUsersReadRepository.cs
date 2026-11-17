using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users.DTO;
using BookStore.Application.Contracts.Users.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Users
{
    public interface IUsersReadRepository
    {
        Task<List<UserShortDTO>> GetAll();
        Task<UserDetailsDTO?> GetById(Guid id);
        Task<UserAuthDTO?> GetByEmail(string email);
        Task<PagedResult<UserShortDTO>> GetPagedAsync(UserQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<bool> IsExistById(Guid id);
        Task<bool> IsExistByEmail(string email);
        Task<bool> HasPermission(Guid userId, string permissionCode);

    }
}
