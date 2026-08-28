using BookStore.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace BookStore.Core.Entities
{
    public class UserEntity
    {
        public const int MAX_NAME_LENGTH = 250;
        public const int MAX_EMAIL_LENGTH = 250;

        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string ProfilePhotoURL { get; private set; } = string.Empty;

        private readonly HashSet<Guid?> _roleIds;
        public IReadOnlyCollection<Guid?> RoleIds => _roleIds;

        //добавить связи с другими моделями

        private UserEntity()
        {
        }

        private UserEntity(Guid id, string name, string email, string passwordHash, string? profilePhotoURL, IEnumerable<Guid?> roleIds)
        {
            Id = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            ProfilePhotoURL = profilePhotoURL;

            _roleIds = new HashSet<Guid?>(roleIds);
        }

        public static (UserEntity User, string Error) Create(Guid id, string name, string email, string passwordHash, string? profilePhotoURL, IEnumerable<Guid?> roleIds)//role добавить
        {
            string error = Validate(id, name, email, passwordHash, profilePhotoURL, roleIds);

            if (!string.IsNullOrEmpty(error))
                return (new UserEntity(), error);

            var userEntity = new UserEntity(id, name, email, passwordHash, profilePhotoURL, roleIds);

            return (userEntity, error);
        }

        private static string Validate(Guid id, string name, string email, string passwordHash, string? profilePhotoURL, IEnumerable<Guid?> roleIds)//role добавить
        {
            string error = string.Empty;
            if (String.IsNullOrEmpty(name) || name.Length > MAX_NAME_LENGTH)
            {
                error = $"Name can not be empty or longer than 250\n";
            }

            if (String.IsNullOrEmpty(email) || email.Length > MAX_EMAIL_LENGTH)
            {
                error = $"Email can not be empty or longer than 250\n";
            }

            if (String.IsNullOrEmpty(passwordHash))
            {
                error = $"Password can not be empty\n";
            }

            if (String.IsNullOrEmpty(profilePhotoURL))
            {
                profilePhotoURL = "https://goo.su/icivqAD";
            }

            return error;
        }

        public void AddRole(Guid roleId)
        {
            _roleIds.Add(roleId);
        }

        public void RemoveRole(Guid roleId)
        {
            _roleIds.Remove(roleId);
        }
    }
}
