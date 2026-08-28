using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class PermissionModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<RoleModel>? Roles { get; set; } = [];

    }
}
