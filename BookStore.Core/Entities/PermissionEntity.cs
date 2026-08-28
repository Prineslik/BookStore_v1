using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BookStore.Core.Entities
{
    public class PermissionEntity
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public string Description { get; private set; } = null!;

        //private readonly HashSet<Guid?> _roleIds;
        public IEnumerable<Guid?> RoleIds;

        public PermissionEntity(Guid id, string code, string description, IEnumerable<Guid?> roleIds)
        {
            Id = id;
            Code = code;
            Description = description;
            RoleIds = roleIds;
        }
    }
}
