using BookStore.Core.Entities;
using BookStore.Core.Exceptions;
using BookStore.Tests.DTO;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BookStore.Tests.Unit.Core.Entities
{
    public class UserEntityTests
    {
        public static readonly UserTestDTO validUserDTO = new(
            Guid.NewGuid(),
                "Valide name",
                "valide.email@mail.ru",
                "valid_password_hash",
                "https://valide-url.com",
                new DateTime(2026,5,12),
                new DateTime(2026, 8, 22),
                true,
                Guid.NewGuid(),
                new List<Guid> { Guid.NewGuid() },
                new List<Guid> { Guid.NewGuid()});

        public static TheoryData<UserTestDTO, string> InvalidUsersAndMessages => new()
        {
            {
                new UserTestDTO(
                validUserDTO.Id,
                null, validUserDTO.Email, validUserDTO.PasswordHash,
                validUserDTO.ProfilePhotoURL, validUserDTO.CreateDate, validUserDTO.UpdateDate,
                validUserDTO.IsAuthor, validUserDTO.AuthorId, validUserDTO.RoleIds, validUserDTO.OwnedBookIds),
                "Name can not be empty"
            },
            {
                new UserTestDTO(
                validUserDTO.Id,
                new string('n',UserEntity.MAX_NAME_LENGTH + 1), validUserDTO.Email, validUserDTO.PasswordHash,
                validUserDTO.ProfilePhotoURL, validUserDTO.CreateDate, validUserDTO.UpdateDate,
                validUserDTO.IsAuthor, validUserDTO.AuthorId, validUserDTO.RoleIds, validUserDTO.OwnedBookIds),
                $"Name can not be longer than {UserEntity.MAX_NAME_LENGTH}"
            },
            {
                new UserTestDTO(
                validUserDTO.Id,
                validUserDTO.UserName, "@incorrect.com", validUserDTO.PasswordHash,
                validUserDTO.ProfilePhotoURL, validUserDTO.CreateDate, validUserDTO.UpdateDate,
                validUserDTO.IsAuthor, validUserDTO.AuthorId, validUserDTO.RoleIds, validUserDTO.OwnedBookIds),
                "Email is incorrect"
            },
            {
                new UserTestDTO(
                validUserDTO.Id,
                validUserDTO.UserName, "", validUserDTO.PasswordHash,
                validUserDTO.ProfilePhotoURL, validUserDTO.CreateDate, validUserDTO.UpdateDate,
                validUserDTO.IsAuthor, validUserDTO.AuthorId, validUserDTO.RoleIds, validUserDTO.OwnedBookIds),
                "Email is incorrect"
            },
            {
                new UserTestDTO(
                validUserDTO.Id,
                validUserDTO.UserName, validUserDTO.Email, "",
                validUserDTO.ProfilePhotoURL, validUserDTO.CreateDate, validUserDTO.UpdateDate,
                validUserDTO.IsAuthor, validUserDTO.AuthorId, validUserDTO.RoleIds, validUserDTO.OwnedBookIds),
                "PasswordHash can not be empty"
            }
        };

        [Theory]
        [MemberData(nameof(InvalidUsersAndMessages))]
        public async Task CreateUser_WhenDataIsInvalid_ShouldThrowDomainException(UserTestDTO userTest, string expectedMessage)
        {
            Action act = () => UserEntity.Create(
                userTest.UserName,
                userTest.Email,
                userTest.PasswordHash,
                userTest.ProfilePhotoURL,
                userTest.AuthorId,
                userTest.RoleIds,
                userTest.OwnedBookIds);

            act.Should()
                .Throw<DomainException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public async Task CreateUser_WhenDateIsValid_ShouldReturnUserEntity()
        {
            var result = UserEntity.Create(
                validUserDTO.UserName,
                validUserDTO.Email,
                validUserDTO.PasswordHash,
                validUserDTO.ProfilePhotoURL,
                validUserDTO.AuthorId,
                validUserDTO.RoleIds,
                validUserDTO.OwnedBookIds);

            result.UserName.Should().Be(validUserDTO.UserName);
            result.Email.Should().Be(validUserDTO.Email);
            result.PasswordHash.Should().Be(validUserDTO.PasswordHash);
            result.AuthorId.Should().Be(validUserDTO.AuthorId);
            //result.RoleIds.Should().Be(roleIds);
            //result.OwnedBookIds.Should().Be(bookIds);
        }
    }
}
