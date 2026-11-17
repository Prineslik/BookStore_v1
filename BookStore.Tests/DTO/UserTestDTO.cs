using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Tests.DTO
{
    public record UserTestDTO(
        Guid? Id,
        string? UserName,
        string? Email,
        string? PasswordHash,
        string? ProfilePhotoURL,
        DateTime? CreateDate,
        DateTime? UpdateDate,
        bool? IsAuthor,
        Guid? AuthorId,
        List<Guid> RoleIds,
        List<Guid> OwnedBookIds);
    
}
