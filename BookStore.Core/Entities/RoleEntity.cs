using BookStore.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Core.Entities
{
    public class RoleEntity
    {
        public const int MAX_NAME_LENGTH = 250;

        public Guid Id { get; private set; }
        public string Name { get; private set; }

        private readonly List<Guid> _permissionIds;
        public IReadOnlyCollection<Guid> PermissionIds => _permissionIds;

        private readonly List<Guid> _userIds;
        public IReadOnlyCollection<Guid> UserIds => _userIds;
        
        private RoleEntity(Guid id, string name, List<Guid> permissionIds, List<Guid> userIds)
        {
            Id = id;
            Name = name;
            _permissionIds = permissionIds;
            _userIds = userIds;
        }

        public static RoleEntity Create(string name, List<Guid> permissionIds/*, List<Guid> userIds*/)
        {
            ValidateName(name);
            ValidatePermissionIds(permissionIds);

            return new RoleEntity(Guid.NewGuid(), name, permissionIds, new List<Guid>());
        }

        public static RoleEntity Reconstitute(Guid id, string name, List<Guid> permissionIds, List<Guid> userIds)
        {
            return new RoleEntity(id, name, permissionIds, userIds);
        }

        public void Rename(string name)
        {
            ValidateName(name);
            Name = name;
        }

        public void AddPermission(Guid permissionId)
        {
            if (_permissionIds.Contains(permissionId))
                return;

            _permissionIds.Add(permissionId);
        }

        public void AddPermissions(List<Guid> permissionIds)
        {
            ValidatePermissionIds(permissionIds);

            _permissionIds.AddRange(permissionIds.Where(p => !_permissionIds.Contains(p)));
        }

        //private void SyncUsers(RoleModel role, IReadOnlyCollection<Guid> usersIds)
        //{
        //    var target = usersIds.ToHashSet();
        //    role.UserRoleModels.RemoveAll(ur => !target.Contains(ur.UserId));
        //    var current = role.UserRoleModels.Select(ur => ur.UserId).ToHashSet();
        //    foreach (var userId in target.Except(current))
        //        role.UserRoleModels.Add(new UserRoleModel
        //        {
        //            UserId = userId,
        //            RoleId = role.Id
        //        });
        //}
        public void SynchPermissions(List<Guid> permissionIds)
        {
            ValidatePermissionIds(permissionIds);

            var target = permissionIds.ToHashSet();
            _permissionIds.RemoveAll(p => !target.Contains(p));
            var current = _permissionIds.ToHashSet();
            foreach (var permissionId in target.Except(current))
                _permissionIds.Add(permissionId);

            //_permissionIds.AddRange(permissionIds.Where(p => !_permissionIds.Contains(p)));
        }

        public void RemovePermission(Guid permissionId)
        {
            if (_permissionIds.Count == 1)
                throw new DomainException("Role cannot remain without permissions");

            _permissionIds.Remove(permissionId);
        }

        private static void ValidateName(string name)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
                throw new DomainException("Name can not be null or empty");
            if (name.Length > MAX_NAME_LENGTH)
                throw new DomainException("Name can not be longer than 250 symbols");
        }

        private static void ValidatePermissionIds(List<Guid> permissionIds)
        { 
            if(permissionIds.Count == 0)
                throw new DomainException("List of permissions can not be empty");
        }

    }
}
