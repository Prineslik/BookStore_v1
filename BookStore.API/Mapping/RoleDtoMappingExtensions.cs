using BookStore.API.Requests.Role;
using BookStore.API.Responses.Role;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Roles.Commands;
using BookStore.Application.Contracts.Roles.DTO;
using System.Runtime.CompilerServices;

namespace BookStore.API.Mapping
{
    public static class RoleDtoMappingExtensions
    {
        public static List<RoleShortResponse> ToResponse(this List<RoleShortDTO> roleDTOs)
            => roleDTOs.Select(r => new RoleShortResponse(r.Id, r.Name)).ToList();

        public static RoleDetailsResponse ToResponse(this RoleDetailsDTO roleDetails)
            => new RoleDetailsResponse(
                roleDetails.Id,
                roleDetails.Name,
                roleDetails.Permissions.ToResponse(),
                roleDetails.Users.ToResponse());

        public static PagedResult<RoleShortResponse> ToResponse(this PagedResult<RoleShortDTO> pagedResult)
            => new PagedResult<RoleShortResponse> 
            {
                Items = pagedResult.Items.ToResponse(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };

        public static RoleUpdateCommand ToCommand(this RoleUpdateRequest updateRequest)
            => new RoleUpdateCommand(
                updateRequest.Id,
                updateRequest.Name,
                updateRequest.PermissionIds);

        public static RoleCreateCommand ToCommand(this RoleCreateRequest createRequest)
            => new RoleCreateCommand(
                createRequest.Name,
                createRequest.PermissionIds);

        public static RoleAddPermissionsCommand ToCommand(this RoleAddPermissionsRequest addPermissionsRequest)
            => new RoleAddPermissionsCommand(
                addPermissionsRequest.Id,
                addPermissionsRequest.PermissionIds);
    }
}
