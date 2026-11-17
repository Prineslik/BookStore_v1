using BookStore.API.Requests.Permission;
using BookStore.API.Responses.Permission;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Permissions.Commands;
using BookStore.Application.Contracts.Permissions.DTO;
using System.Runtime.CompilerServices;

namespace BookStore.API.Mapping
{
    public static class PermissionDtoMappingExtension
    {
        public static List<PermissionShortResponse> ToResponse(this List<PermissionShortDTO> listDTOs)
            => listDTOs.Select(p => new PermissionShortResponse(
                p.Id,
                p.Code,
                p.Description))
            .ToList();

        public static PermissionDetailsResponse ToResponse(this PermissionDetailsDTO detailsDTO)
            => new PermissionDetailsResponse(
                detailsDTO.Id,
                detailsDTO.Code,
                detailsDTO.Description,
                detailsDTO.Roles.ToResponse());

        public static PagedResult<PermissionShortResponse> ToResponse(this PagedResult<PermissionShortDTO> pagedResult)
            => new PagedResult<PermissionShortResponse>
            { 
                Items = pagedResult.Items.ToResponse(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };

        public static PermissionCreateCommand ToCommand(this PermissionCreateRequest createRequest)
            => new PermissionCreateCommand(
                createRequest.Code,
                createRequest.Description);

        public static PermissionUpdateCommand ToCommand(this PermissionUpdateRequest updateRequest)
            => new PermissionUpdateCommand(
                updateRequest.Id,
                updateRequest.Code,
                updateRequest.Description);
    }
}
