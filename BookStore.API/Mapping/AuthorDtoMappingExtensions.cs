using BookStore.API.Responses.User;
using BookStore.Application.Contracts.Authors.DTO;

namespace BookStore.API.Mapping
{
    public static class AuthorDtoMappingExtensions
    {
        public static List<AuthorShortResponse> ToResponse(this List<AuthorShortDTO> shortDTOs)
            => shortDTOs.Select(a => new AuthorShortResponse(
                a.Id,
                a.Name,
                a.ProfilePhotoURL))
            .ToList();
    }
}
