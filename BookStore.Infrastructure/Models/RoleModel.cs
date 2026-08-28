using BookStore.Core.Entities;
using BookStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Models
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UserModel?> Users { get; set; } = [];
        public List<PermissionModel?> Permissions { get; set; } = [];
    }
}
