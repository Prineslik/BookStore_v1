using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles.DTO;
using BookStore.Application.Contracts.Roles.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Roles
{
    public interface IRolesReadRepository
    {
        Task<List<RoleShortDTO>> GetAll();
        Task<RoleDetailsDTO> GetById(Guid id);
        Task<List<RoleShortDTO>> GetByName(string name);
        //Task<RolesResponse?> GetByNameStrict(string name);
        Task<List<RoleShortDTO>> GetByList(List<Guid> ids);
        Task<List<RoleShortDTO>> GetByUser(Guid id);
        Task<bool> IsExistById(Guid id);
        Task<bool> IsExistByName(string name);
        Task<PagedResult<RoleShortDTO>> GetPaged(RoleQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> GetDefaultRole();
    }
}
