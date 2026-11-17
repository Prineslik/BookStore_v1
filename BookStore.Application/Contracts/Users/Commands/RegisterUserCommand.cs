using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Users.Commands
{
    public record RegisterUserCommand(
        string Name,
        string Email,
        string Password);
}
