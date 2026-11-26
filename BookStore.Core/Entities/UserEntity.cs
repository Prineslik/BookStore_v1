using BookStore.Core.Entities;
using BookStore.Core.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BookStore.Core.Entities
{
    public class UserEntity
    {
        public const int MAX_NAME_LENGTH = 250;
        public const int MAX_EMAIL_LENGTH = 250;
        private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }// VO
        public string PasswordHash { get; private set; }
        public string ProfilePhotoURL { get; private set; } = string.Empty;
        public DateTime CreateDate { get; private set; }
        public DateTime? UpdateDate { get; private set; }
        public bool IsAuthor => AuthorId.HasValue && AuthorId.Value != Guid.Empty;

        public Guid? AuthorId { get; private set; }

        private readonly List<Guid> _roleIds;
        public IReadOnlyCollection<Guid> RoleIds => _roleIds;

        private readonly List<Guid> _ownedBookIds;
        public IReadOnlyCollection<Guid> OwnedBookIds => _ownedBookIds;

        private UserEntity() { }

        private UserEntity(
            Guid id,
            string name,
            string email,
            string passwordHash,
            string? profilePhotoURL,
            DateTime createDate,
            DateTime? updateDate,
            Guid? authorId,
            List<Guid> roleIds,
            List<Guid> ownedBookIds)
        {
            Id = id;
            UserName = name;
            Email = email;
            PasswordHash = passwordHash;
            ProfilePhotoURL = profilePhotoURL;
            CreateDate = createDate;
            UpdateDate = updateDate;
            AuthorId = authorId;

            _roleIds = new List<Guid>(roleIds);
            _ownedBookIds = new List<Guid>(ownedBookIds);
        }

        public static UserEntity Create(
            string name,
            string email,
            string passwordHash,
            string? profilePhotoURL,
            Guid? authorId,
            List<Guid> roleIds,
            List<Guid> ownedBookIds)
        {
            ValidateName(name);
            ValidateEmail(email);
            ValidatePasswordHash(passwordHash);
            //ValidateProfilePhotoUrl(profilePhotoURL);
            //ValidateOwnedBookIds(ownedBookIds);

            if (String.IsNullOrEmpty(profilePhotoURL) || String.IsNullOrWhiteSpace(profilePhotoURL))
                profilePhotoURL = "https://goo.su/icivqAD";



            var userEntity = new UserEntity(
                Guid.NewGuid(),
                name,
                email,
                passwordHash,
                profilePhotoURL,
                DateTime.UtcNow,
                null,
                authorId,
                roleIds,
                ownedBookIds);

            return userEntity;
        }

        public static UserEntity Reconstitute(
            Guid id,
            string name,
            string email,
            string passwordHash,
            string? profilePhotoURL,
            DateTime createDate,
            DateTime? updateDate,
            Guid? authorId,
            List<Guid> roleIds,
            List<Guid> ownedBookIds)
        {
            return new UserEntity(
                id,
                name,
                email,
                passwordHash,
                profilePhotoURL,
                createDate,
                updateDate,
                authorId,
                roleIds,
                ownedBookIds);
        }

        public void Rename(string name)
        {
            ValidateName(name);
            UserName = name;
        }

        public void ChangeEmail(string email)
        {
            ValidateEmail(email);
            Email = email;
        }

        public void ChangePasswordHash(string passwordHash)
        {
            ValidatePasswordHash(passwordHash);
            PasswordHash = passwordHash;
        }

        public void ChangeProfilePhotoUrl(string profilePhotoUrl)
        {
            ValidateProfilePhotoUrl(profilePhotoUrl);
            ProfilePhotoURL = profilePhotoUrl;
        }

        public void AddRole(Guid roleId)
        {
            if (_roleIds.Contains(roleId))
                return;

            _roleIds.Add(roleId);
            UpdateDate = DateTime.UtcNow;
        }

        public void RemoveRole(Guid roleId)
        {
            _roleIds.Remove(roleId);
            UpdateDate = DateTime.UtcNow;
        }

        public void AddOwnedBook(Guid ownedBookId)
        {
            if (_ownedBookIds.Contains(ownedBookId))
                return;

            _ownedBookIds.Add(ownedBookId);
            UpdateDate = DateTime.UtcNow;
        }

        public void RemoveOwnedBook(Guid ownedBookId)
        {
            _ownedBookIds.Remove(ownedBookId);
            UpdateDate = DateTime.UtcNow;
        }

        private static void ValidateProfilePhotoUrl(string profilePhotoUrl)
        {
            if (String.IsNullOrWhiteSpace(profilePhotoUrl) || String.IsNullOrEmpty(profilePhotoUrl))
                throw new DomainException("Profile photo url can not be empty");
        }

        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
                throw new DomainException("Email is incorrect");
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new DomainException($"Name can not be empty");

            if (name.Length > MAX_NAME_LENGTH)
                throw new DomainException($"Name can not be longer than {MAX_NAME_LENGTH}");
        }

        private static void ValidatePasswordHash(string passwordHash)
        {
            if (String.IsNullOrEmpty(passwordHash) || String.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("PasswordHash can not be empty");
            
        }

        private static void ValidateOwnedBookIds(List<Guid> ownedBookIds)
        {
            if (ownedBookIds.Count == 0)
                throw new DomainException("List of owned books cannot be empty");
        }
    }
}
