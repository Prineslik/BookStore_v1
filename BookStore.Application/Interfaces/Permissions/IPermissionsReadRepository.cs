using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions.DTO;
using BookStore.Application.Contracts.Permissions.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Permissions
{
    public interface IPermissionsReadRepository
    {
        Task<List<PermissionShortDTO>> GetAll();
        Task<PermissionDetailsDTO> GetById(Guid id);
        Task<List<PermissionShortDTO>> GetByCode(string code);
        Task<bool> IsExistById(Guid id);
        Task<bool> IsExistByCode(string code);
        Task<PagedResult<PermissionShortDTO>> GetPaged(PermissionQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
    }
}
