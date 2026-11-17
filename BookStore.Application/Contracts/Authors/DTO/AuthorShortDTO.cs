using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Authors.DTO
{
    public record AuthorShortDTO(
        Guid Id,
        string Name,
        string ProfilePhotoURL
    );
}
