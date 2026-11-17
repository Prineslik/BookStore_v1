using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class UserRoleModel
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public UserModel User { get; set; } = null!;
        public RoleModel Role { get; set; } = null!;
    }
}
