using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookStore.API.Requests.User
{
    public record class LoginUserRequest(
        string Email,
        string Password);
}
