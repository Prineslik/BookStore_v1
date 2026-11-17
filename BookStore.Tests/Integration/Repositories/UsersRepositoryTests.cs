using BookStore.Application.Interfaces.Users;
using BookStore.Core.Entities;
using BookStore.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BookStore.Tests.Integration.Repositories
{
    public class UsersRepositoryTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private IServiceScope _scope;
        private IUsersRepository _usersRepository;
        private IUsersReadRepository _usersReadRepository;

        public UsersRepositoryTests(IntegrationTestWebAppFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
            _scope = _factory.Services.CreateScope();
            _usersRepository = _scope.ServiceProvider.GetRequiredService<IUsersRepository>();
            _usersReadRepository = _scope.ServiceProvider.GetRequiredService<IUsersReadRepository>();
        }

        public Task DisposeAsync()
        {
            _scope.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            var users = await _usersReadRepository.GetAll();

            users.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldSaveUser()
        {
            var newUser = UserEntity.Create(                
                "John Doe",
                "john@mail.com",
                "hash123",
                "https://photo.jpg",
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                new List<Guid>(),
                new List<Guid>()
            );

            var userId = await _usersRepository.Create(newUser);

            userId.Should().Be(newUser.Id);
        }

        [Fact]
        public async Task GetByEmail_WhenEmailIsNotExist_ShouldReturnNullUser()
        {
            string email = "notfound@test.com";
            var user = await _usersReadRepository.GetByEmail(email);

            user.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmail_WhenEmailIsExist_ShouldReturnUser()
        {
            string email = "testauthor@example.com";

            var user = await _usersReadRepository.GetByEmail(email);
            
            user.Email.Should().Be(email);
        }

        [Fact]
        public async Task GetById_ShouldReturnUser()
        {
            Guid id = new Guid("11111111-1111-1111-1111-111111111111");

            var user = await _usersRepository.GetById(id);

            user.Email.Should().Be("testuser@example.com");
        }

        [Fact]
        public async Task Update_WhenDataIsValid_ShouldReturnUpdatedUser()
        {
            var newUserEntity = UserEntity.Reconstitute(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "updated name",
                "updated_email@mail.com",
                "password_hash",
                "https://new-photo.png",
                new DateTime(2026,8,15),
                DateTime.UtcNow,
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                new List<Guid>() { Guid.Parse("33333333-3333-3333-3333-333333333333") },
                new List<Guid>() 
                { 
                    Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Guid.Parse("99999999-9999-9999-9999-999999999999")
                });

            var updatedUserId = await _usersRepository.Update(newUserEntity);

            updatedUserId.Should().Be(newUserEntity.Id);

            //var updatedResult = await _usersRepository.GetById(updatedUserId);
            //updatedResult.Should().BeEquivalentTo(Arg.Is<UserEntity>(u =>
            //u.Id == newUserEntity.Id &&
            //u.UserName == newUserEntity.UserName &&
            //u.Email == newUserEntity.Email &&
            //u.ProfilePhotoURL == newUserEntity.ProfilePhotoURL && 
            //u.PasswordHash == newUserEntity.PasswordHash &&
            //u.CreateDate == newUserEntity.CreateDate &&
            //u.UpdateDate == newUserEntity.UpdateDate &&
            //u.AuthorId == newUserEntity.AuthorId &&
            //u.IsAuthor == newUserEntity.IsAuthor &&
            //u.RoleIds.Equals(newUserEntity.RoleIds) &&
            //u.OwnedBookIds.Equals(newUserEntity.OwnedBookIds)
            //));
        }
        [Fact]
        public async Task Update_WhenDataIsNotValid_ShouldThrowInvalidOperationException()
        {
            var newUserEntity = UserEntity.Reconstitute(
                Guid.Parse("11111222-3333-4444-5555-111111111111"),
                "updated name",
                "updated_email@mail.com",
                "password_hash",
                "https://new-photo.png",
                new DateTime(2026,8,15),
                DateTime.UtcNow,
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                new List<Guid>() { Guid.Parse("33333333-3333-3333-3333-333333333333") },
                new List<Guid>() 
                { 
                    Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Guid.Parse("99999999-9999-9999-9999-999999999999")
                });

            Func<Task> act = async () => await _usersRepository.Update(newUserEntity);

            await act.Should().ThrowAsync<InvalidOperationException>();

            //updatedUserId.Should().Be(newUserEntity.Id);

            //var updatedResult = await _usersRepository.GetById(updatedUserId);
            //updatedResult.Should().BeEquivalentTo(Arg.Is<UserEntity>(u =>
            //u.Id == newUserEntity.Id &&
            //u.UserName == newUserEntity.UserName &&
            //u.Email == newUserEntity.Email &&
            //u.ProfilePhotoURL == newUserEntity.ProfilePhotoURL && 
            //u.PasswordHash == newUserEntity.PasswordHash &&
            //u.CreateDate == newUserEntity.CreateDate &&
            //u.UpdateDate == newUserEntity.UpdateDate &&
            //u.AuthorId == newUserEntity.AuthorId &&
            //u.IsAuthor == newUserEntity.IsAuthor &&
            //u.RoleIds.Equals(newUserEntity.RoleIds) &&
            //u.OwnedBookIds.Equals(newUserEntity.OwnedBookIds)
            //));
        }


    }
}
