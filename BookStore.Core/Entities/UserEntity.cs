using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace BookStore.Core.Models
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
        //добавить связи с другими моделями

        private UserEntity()
        {
            
        }

        private UserEntity(Guid id, string name, string email, string passwordHash, string? profilePhotoURL)
        {
            Id = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            ProfilePhotoURL = profilePhotoURL;
        }

        public static (UserEntity User, string Error) Create(Guid id, string name, string email, string passwordHash, string? profilePhotoURL)
        {
            var userEntity = new UserEntity(id, name, email, passwordHash, profilePhotoURL);

            string error = Validate(userEntity);

            return (userEntity, error);
        }

        //public static (UserEntity User, string Error) Create(string name, string email, string password, string? profilePhotoURL)
        //{
        //    var userEntity = new UserEntity(Guid.NewGuid(), name, email, password, profilePhotoURL);

        //    string error = Validate(userEntity);

        //    return (userEntity, error);
        //}

        private static string Validate(UserEntity user)
        {
            string error = string.Empty;
            if (String.IsNullOrEmpty(user.Name) || user.Name.Length > MAX_NAME_LENGTH)
            {
                error = $"Name can not be empty or longer than 250\n";
            }

            if (String.IsNullOrEmpty(user.Email) || user.Email.Length > MAX_EMAIL_LENGTH)
            {
                error = $"Email can not be empty or longer than 250\n";
            }

            if (String.IsNullOrEmpty(user.PasswordHash))
            {
                error = $"Password can not be empty\n";
            }

            if (String.IsNullOrEmpty(user.ProfilePhotoURL))
            {
                user.ProfilePhotoURL = "https://goo.su/icivqAD";
            }

            return error;
        }
    }
}
