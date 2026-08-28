using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Core.Entities
{
    public class RoleEntity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        // Инкапсулированная коллекция ID прав
        private readonly HashSet<Guid?> _permissionIds;
        public IReadOnlyCollection<Guid?> PermissionIds => _permissionIds;
        
        private RoleEntity(Guid id, string name, IEnumerable<Guid?> permissionIds)
        {
            Id = id;
            Name = name;
            _permissionIds = new HashSet<Guid?>(permissionIds);
        }

        public static (RoleEntity RoleEntity, string Error) Create(Guid id, string name, IEnumerable<Guid?> permissionIds)
        {
            string Error = string.Empty;

            return (new RoleEntity(id, name, permissionIds), Error);
        }

        // Бизнес-логика динамического редактирования
        public void AddPermission(Guid permissionId)
        {
            _permissionIds.Add(permissionId);
        }

        public void AddPermissions(IEnumerable<Guid?> permissionIds)
        {
            _permissionIds.UnionWith(permissionIds);
        }

        public void RemovePermission(Guid permissionId)
        {
            _permissionIds.Remove(permissionId);
        }
    }
}
