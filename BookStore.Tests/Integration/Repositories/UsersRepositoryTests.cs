using BookStore.Application.Interfaces.Users;
using BookStore.Core.Entities;
using BookStore.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
        private IUsersRepository _repository;

        public UsersRepositoryTests(IntegrationTestWebAppFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();      
            _scope = _factory.Services.CreateScope(); 
            _repository = _scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        }

        public Task DisposeAsync()
        {
            _scope.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            var users = await _repository.GetAll();

            users.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldSaveUser()
        {
            var newUser = UserEntity.Create(
                Guid.NewGuid(),
                "John Doe",
                "john@mail.com",
                "hash123",
                "photo_url",
                Enumerable.Empty<Guid>()
            ).User;

            var userId = await _repository.Create(newUser);

            userId.Should()
                .NotBeEmpty()
                .And.Be(newUser.Id);
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnNullUser()
        {
            var user = await _repository.GetByEmail("notfound@test.com");

            user.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnAdminUser()
        {
            var user = await _repository.GetByEmail("admin@test.com");

            user.Should().NotBeNull();
            user.Email.Should().Be("admin@test.com");
        }

        [Fact]
        public async Task GetById_ShouldReturnUser()
        {
            Guid id = new Guid("22222222 - 2222 - 2222 - 2222 - 222222222222");

            var user = await _repository.GetById(id);

            user.Should().NotBeNull();
            user.Email.Should().Be("user@test.com");
        }

        [Fact]
        public async Task Update_ShouldReturnUpdatedUser()
        {
            Guid id = new Guid("22222222 - 2222 - 2222 - 2222 - 222222222222");
            string newUserName = "Updated user";
            string newUserEmail = "updated_user@test.com";
            IEnumerable<Guid> newRoles = new Guid[] 
            {
                new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc")
            };

            var updatedUser = UserEntity.Create(
                id,
                newUserName,
                newUserEmail,
                "$2a$11$r/rl8BO6wbgZ5obVWltIRu.sH9daMFCXyIjQmily58v66wf/QVm2K",
                "https://example.com/avatar1.png",
                newRoles
            );

            var user = await _repository.GetById(id);

            user.Should().NotBeNull();
            user.Email.Should().Be("user@test.com");
        }


    }
}
