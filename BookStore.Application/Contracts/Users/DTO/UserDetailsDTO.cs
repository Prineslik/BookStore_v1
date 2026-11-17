using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Roles.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Users.DTO
{
    public record UserDetailsDTO(
        Guid Id,
        string Name,
        string Email,
        string ProfilePhotoURL,
        bool IsAuthor,
        Guid? AuthorId,
        List<RoleShortDTO> Roles,
        List<BookShortDTO> OwnedBooks
    );
}
