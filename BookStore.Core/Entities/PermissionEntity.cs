using BookStore.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace BookStore.Core.Entities
{
    public class PermissionEntity
    {
        public const int MAX_CODE_LENGTH = 250;

        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public string Description { get; private set; } = null!;

        private readonly List<Guid> _roleIds = new();
        public IReadOnlyList<Guid> RoleIds => _roleIds;

        private PermissionEntity(Guid id, string code, string description, List<Guid> roleIds)
        {
            Id = id;
            Code = code;
            Description = description;
            _roleIds = roleIds;
        }

        public static PermissionEntity Create(string code, string description)
        {
            ValidateCode(code);
            ValidateDescription(description);

            return new PermissionEntity(Guid.NewGuid(), code, description, new List<Guid>());
        }

        public static PermissionEntity Reconstitute(Guid id, string code, string description, List<Guid> roleIds)
        {
            return new PermissionEntity(id, code, description, roleIds);
        }

        public void ChangeCode(string newCode)
        {
            if (Code == newCode) return;

            ValidateCode(newCode);
            Code = newCode.Trim();
        }
        public void ChangeDescription(string newDescription)
        {
            if (Description == newDescription) return;

            ValidateDescription(newDescription);
            Description = newDescription.Trim();
        }

        public void AddRole(Guid roleId)
        {
            if (_roleIds.Contains(roleId)) return;

            //ValidateRoleCanBeAdded(roleId);
            _roleIds.Add(roleId);
        }

        private static void ValidateCode(string code)
        {
            if (String.IsNullOrEmpty(code))
                throw new DomainException("Code can not be null or empty.");
            if (code.Length > MAX_CODE_LENGTH)
                throw new DomainException($"Code cannot exceed {MAX_CODE_LENGTH} characters.");
        }

        private static void ValidateDescription(string description)
        {
            if (description.Length > 100)
                throw new DomainException("Description cannot exceed 100 characters.");
        }
        
        //private void ValidateRoleCanBeAdded(Guid roleId)
        //{
        //    if (_roleIds.Contains(roleId))
        //        throw new Exception
        //}

    }
}
