using BookStore.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class UserModel : IdentityUser
    {
        public new Guid Id { get; set; }
        public string ProfilePhotoURL { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsAuthor { get; set; } = false;
        public Guid? AuthorId { get; set; }

        public AuthorModel? Author { get; set; }
        public List<UserRoleModel> UserRoleModels { get; set; } = [];
        public List<UserBookModel> UserBookModels { get; set; } = [];

    }
}
