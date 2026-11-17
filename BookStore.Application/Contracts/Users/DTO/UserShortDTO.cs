using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Users.DTO
{
    public record UserShortDTO(
        Guid Id,
        string Name,
        string Email,
        string ProfilePhotoURL,
        bool IsAuthor
    );
}
