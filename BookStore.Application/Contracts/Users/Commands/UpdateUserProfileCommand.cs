using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Users.Commands
{
    public record UpdateUserProfileCommand(
        Guid Id,
        string Name,
        string Email,
        string ProfilePhotoURL
    );
}
