using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Core.Models
{
    public class UserEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? ProfilePhotoURL { get; set; }

        //добавить связи с другими моделями
    }
}
