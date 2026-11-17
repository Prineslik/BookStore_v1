using BookStore.API.Responses.User;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users.DTO;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookStore.API.Mapping
{
    public static class UserDtoMappingExtensions
    {
        public static List<UserShortResponse> ToResponse (this List<UserShortDTO> listDTOs)
            => listDTOs.Select(u => new UserShortResponse(
                u.Id,
                u.Name,
                u.Email,
                u.ProfilePhotoURL,
                u.IsAuthor))
            .ToList();

        public static List<OwnerShortResponse> ToResponse (this List<OwnerShortDTO> listDTOs)
            => listDTOs.Select(u => new OwnerShortResponse(
                u.Id,
                u.Name,
                u.Email,
                u.ProfilePhotoURL,
                u.IsAuthor))
            .ToList();

        public static UsersDetailsResponse ToResponse(this UserDetailsDTO detailsDTO)
            => new UsersDetailsResponse(
                detailsDTO.Id,
                detailsDTO.Name,
                detailsDTO.Email,
                detailsDTO.ProfilePhotoURL,
                detailsDTO.IsAuthor,
                detailsDTO.AuthorId,
                detailsDTO.Roles.ToResponse(),
                detailsDTO.OwnedBooks.ToResponse());
        
        public static PagedResult<UserShortResponse> ToResponse(this PagedResult<UserShortDTO> pagedResult)
            => new PagedResult<UserShortResponse>()
            {
                Items = pagedResult.Items.ToResponse(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };

        //public static UserA ToResponse(this UserDetailsDTO detailsDTO)
        //    => new UsersDetailsResponse(
        //        detailsDTO.Id,
        //        detailsDTO.Name,
        //        detailsDTO.Email,
        //        detailsDTO.ProfilePhotoURL,
        //        detailsDTO.IsAuthor,
        //        detailsDTO.AuthorId,
        //        detailsDTO.Roles.ToResponse(),
        //        detailsDTO.OwnedBooks.ToResponse());
    }
}
