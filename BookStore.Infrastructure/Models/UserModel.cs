using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class UserModel : IdentityUser
    {
        //public UserModel(Guid id, string name, string email, string passwordHash, string? profilePhotoURL) : base()
        //{
        //    Id = id.ToString();
        //    UserName = name;
        //    Email = email;
        //    ProfilePhotoURL = profilePhotoURL;
        //}
        public new Guid Id { get; set; }
        public string ProfilePhotoURL { get; private set; } = string.Empty;

    }
}
