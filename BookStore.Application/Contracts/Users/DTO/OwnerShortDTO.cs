using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Users.DTO
{
    public record OwnerShortDTO(
        Guid Id,
        string Name,
        //string Email,
        string ProfilePhotoURL,
        bool IsAuthor);
}
