using BookStore.API.Responses.Book;
using BookStore.API.Responses.Role;

namespace BookStore.API.Responses.User
{
    public record UsersDetailsResponse
    (
        Guid Id,
        string Name,
        string Email,
        string ProfilePhotoURL,
        bool IsAuthor,
        Guid? AuthorId,
        List<RoleShortResponse> Roles,
        List<BookShortResponse> OwnedBooks
    );
}
