using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class PermissionRoleModel
    {
        public Guid PermissionId { get; set; }
        public Guid RoleId { get; set; }

        public PermissionModel Permission { get; set; } = null!;
        public RoleModel Role { get; set; } = null!;
    }
}
