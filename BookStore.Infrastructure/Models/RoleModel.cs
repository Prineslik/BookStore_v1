using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<UserRoleModel> UserRoleModels { get; set; } = [];
        public List<PermissionRoleModel> PermissionRoleModels { get; set; } = [];
    }
}
