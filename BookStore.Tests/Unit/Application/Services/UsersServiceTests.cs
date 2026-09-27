using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Roles;
using BookStore.Application.Interfaces.Users;
using BookStore.Application.Services;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Repositories;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Xunit;

namespace BookStore.Tests.Unit.Application.Services
{
    public class UsersServiceTests
    {
        private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
        private readonly IRolesRepository _rolesRepository = Substitute.For<IRolesRepository>();
        private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
        private readonly IJwtProvider _jwtProvider = Substitute.For<IJwtProvider>();

        private readonly UsersService _usersService;

        public UsersServiceTests()
        {
            _usersService = new UsersService(_usersRepository, _passwordHasher, _jwtProvider, _rolesRepository);
        }

        [Fact]
        public async Task LoginUser_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange (Подготовка)
            var name = "try login";
            var email = "try@mail.com";
            var password = "123";
            var hashedPassword = "$2a$11$Z3REWMzHnBNTezRVPRNzYeOySW4m7ibHjqo9yqTztlg8F5Dezf9Eu";
            var profilePhotoURL = "https://goo.su/icivqAD";
            List<RoleEntity> roles = new List<RoleEntity>();
            var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2MDhmOWM4ZC1lNzk5LTQ0ZGUtOWFhNi0wZjYyZmY0ZWQ5MmQiLCJleHAiOjE3ODg5MDg3NTd9.rADapGVw7CxbBjSwBmAqas8j82tPUbglxrhcPdgxGFk";

            var user = UserEntity.Create(Guid.NewGuid(), name, email, hashedPassword, profilePhotoURL, Enumerable.Empty<Guid>()).User;

            // Настраиваем репозиторий: вернуть пользователя по email
            _usersRepository.GetByEmail(email).Returns(user);

            // Настраиваем хэшер: если пароль и хэш совпадают, вернуть true
            _passwordHasher.Verify(password, hashedPassword).Returns(true);

            // Настраиваем JWT: при передаче нашего пользователя вернуть токен
            _jwtProvider.GenerateToken(Arg.Any<UserEntity>(), Arg.Any<List<RoleEntity>>()).Returns(expectedToken);

            // Act (Выполнение)
            var result = await _usersService.LoginUser(email, password);

            // Assert (Проверки результатов)
            result.Should().NotBeNull();
            result.Should().Be(expectedToken);

            // Assert (Проверки вызовов — на всякий случай)
            _passwordHasher.Received(1).Verify(password, hashedPassword);
            _jwtProvider.Received(1).GenerateToken(Arg.Any<UserEntity>(), Arg.Any<List<RoleEntity>>());
        }

        [Fact]
        public async Task LoginUser_WithWrongPassword_ShouldThrowUnauthorizedException()
        {
            var name = "try login";
            var email = "try@mail.com";
            var password = "123";
            var hashedPassword = "$2a$11$Z3REWMzHnBNTezRVPRNzYeOySW4m7ibHjqo9yqTztlg8F5Dezf9Eu";
            var profilePhotoURL = "https://goo.su/icivqAD";
            
            var user = UserEntity.Create(Guid.NewGuid(), name, email, hashedPassword, profilePhotoURL, Enumerable.Empty<Guid>()).User;

            _usersRepository.GetByEmail(email).Returns(user);

            _passwordHasher.Verify(password, hashedPassword).Returns(false);

            Func<Task> act = async () => await _usersService.LoginUser(email, password);

            await act.Should()
                .ThrowAsync<UnauthorizedException>()
                .WithMessage("Пользователь ввел неправильный email или пароль");

            _passwordHasher.Received(1).Verify(password, hashedPassword);
            _jwtProvider.DidNotReceive().GenerateToken(Arg.Any<UserEntity>(), Arg.Any<List<RoleEntity>>());
        }

        [Fact]
        public async Task LoginUser_WithWrongEmail_ShouldThrowUnauthorizedException()
        {
            var name = "try login";
            var email = "try@mail.com";
            var password = "123";
            var hashedPassword = "$2a$11$Z3REWMzHnBNTezRVPRNzYeOySW4m7ibHjqo9yqTztlg8F5Dezf9Eu";
            var profilePhotoURL = "https://goo.su/icivqAD";

            var user = UserEntity.Create(Guid.NewGuid(), name, email, hashedPassword, profilePhotoURL, Enumerable.Empty<Guid>()).User;

            _usersRepository.GetByEmail(email).Returns((UserEntity)null!);

            //_passwordHasher.Verify(password, hashedPassword).Returns(false);

            Func<Task> act = async () => await _usersService.LoginUser(email, password);

            await act.Should()
                .ThrowAsync<UnauthorizedException>()
                .WithMessage("Пользователь ввел неправильный email или пароль");

            _passwordHasher.DidNotReceive().Verify(password, hashedPassword);
            _jwtProvider.DidNotReceive().GenerateToken(Arg.Any<UserEntity>(), Arg.Any<List<RoleEntity>>());
        }

        [Fact]
        public async Task CreateUser_WithValidUserEntity_ShouldReturnUserId()
        {
            var name = "try login";
            var email = "try@mail.com";
            var password = "123";
            var hashedPassword = "$2a$11$Z3REWMzHnBNTezRVPRNzYeOySW4m7ibHjqo9yqTztlg8F5Dezf9Eu";
            var profilePhotoURL = "https://goo.su/icivqAD";
            List<Guid> roleIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            var userRequset = new UsersRequest(Guid.NewGuid(), name, email, password, profilePhotoURL, roleIds);
            var userEntity
            _usersRepository.Create(user).Returns(user.Id);

            var result = await _usersService.CreateUser(user);

            result.Should()
                .NotBeEmpty()
                .And.Be(user.Id);
            //result.Should().Be(user.Id);
        }

        [Fact]
        public async Task CreateUser_WithDuplicatedEmail_ShouldThrowDuplicatedException()
        {
            var name = "try login";
            var email = "try@mail.com";
            var password = "123";
            var hashedPassword = "$2a$11$Z3REWMzHnBNTezRVPRNzYeOySW4m7ibHjqo9yqTztlg8F5Dezf9Eu";
            var profilePhotoURL = "https://goo.su/icivqAD";

            var user = UserEntity.Create(Guid.NewGuid(), name, email, hashedPassword, profilePhotoURL, Enumerable.Empty<Guid>()).User;

            _usersRepository.Create(user).ThrowsAsync(new DuplicateException(""));//.Returns(user.Id);

            Func<Task> act = async () => await _usersService.CreateUser(user);

            await act.Should().ThrowAsync<DuplicateException>();
        }

        [Fact]
        public async Task UpdateUser_WithValidUserEntity_ShouldReturnUserId()
        {
            var name = "try login";
            var email = "try@mail.com";
            var password = "123";
            var hashedPassword = "$2a$11$Z3REWMzHnBNTezRVPRNzYeOySW4m7ibHjqo9yqTztlg8F5Dezf9Eu";
            var profilePhotoURL = "https://goo.su/icivqAD";
            var roleIds = new Guid[]
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            List<RoleEntity> expectedRoles = new List<RoleEntity>()
            {
                RoleEntity.Create(roleIds[0],"1", Enumerable.Empty<Guid>()).RoleEntity,
                RoleEntity.Create(roleIds[1],"2", Enumerable.Empty<Guid>()).RoleEntity
            };

            var user = UserEntity.Create(Guid.NewGuid(), name, email, hashedPassword, profilePhotoURL, roleIds).User;

            _usersRepository.Update(user).Returns(user.Id);

            _rolesRepository.GetByList(Arg.Any<List<Guid>>()).Returns(expectedRoles);

            var result = await _usersService.UpdateUser(user);

            result.Should()
                .NotBeEmpty()
                .And.Be(user.Id);

            _usersRepository.Received(1).Update(user);
            _rolesRepository.Received(1).GetByList(Arg.Any<List<Guid>>());
        }

        [Fact]
        public async Task DeleteUser_WithExistedId_ShouldReturnUserId()
        {
            Guid userId = Guid.NewGuid();

            _usersRepository.Delete(userId).Returns(userId);

            var result = await _usersService.DeleteUser(userId);

            //await act.Should().ThrowAsync<InfrastructureException>();
            result.Should().Be(userId);
        }

        [Fact]
        public async Task DeleteUser_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            Guid userId = Guid.NewGuid();

            _usersRepository.Delete(userId).ThrowsAsync(new InfrastructureException($"Failed to delete user with GUID = {userId}"));

            Func<Task> act = async () => await _usersService.DeleteUser(userId);

            await act.Should().ThrowAsync<InfrastructureException>();
            //result.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnUsers()
        {
            var users = new List<UserEntity?> 
            {
                UserEntity.Create(Guid.NewGuid(), "User 1", "user1@mail.com", "hash", "photo", Enumerable.Empty<Guid>()).User,
                UserEntity.Create(Guid.NewGuid(), "User 2", "user2@mail.com", "hash", "photo", Enumerable.Empty<Guid>()).User
            };

            _usersRepository.GetAll().Returns(users);

            var result = await _usersService.GetAllUsers();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(users);

            await _usersRepository.Received(1).GetAll();
        }

        [Fact]
        public async Task GetPagedUsersAsync_WithValidQueryParameters_ShuoldReturnPagedResult()
        {
            var userQueryParameters = new UserQueryParameters() { SearchTerm = "" };

            var expectedUsers = new List<UserEntity?>
            {
                UserEntity.Create(Guid.NewGuid(), "User 1", "user1@mail.com", "hash", "photo", Enumerable.Empty<Guid>()).User,
                UserEntity.Create(Guid.NewGuid(), "User 2", "user2@mail.com", "hash", "photo", Enumerable.Empty<Guid>()).User
            };

            var expectedPagedResult = new PagedResult<UserEntity>()
            {
                Items = expectedUsers,
                TotalCount = expectedUsers.Count(),
                PageNumber = userQueryParameters.PageNumber,
                PageSize = userQueryParameters.PageSize,

            };

            _usersRepository.GetPagedAsync(userQueryParameters).Returns(expectedPagedResult);

            var result = await _usersService.GetPagedUsersAsync(userQueryParameters);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPagedResult);

            await _usersRepository.Received(1).GetPagedAsync(Arg.Any<UserQueryParameters>());
        }

        [Fact]
        public async Task GetUserById_WhenUserExists_ShouldReturnUser()
        {
            var userId = Guid.NewGuid();
            var expectedUser = UserEntity.Create(
                userId,
                "John Doe",
                "john@mail.com",
                "hash123",
                "photo_url",
                Enumerable.Empty<Guid>()
            ).User;

            _usersRepository.GetById(userId).Returns(expectedUser);

            var result = await _usersService.GetUserById(userId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUser);

            await _usersRepository.Received(1).GetById(userId);
        }

        [Fact]
        public async Task GetUserById_WhenUserDoesNotExist_ShouldThrowNotFoundException()
        {
            var nonExistingId = Guid.NewGuid();

            _usersRepository.GetById(nonExistingId).ThrowsAsync(new NotFoundException("User", nonExistingId));

            Func<Task> act = async () => await _usersService.GetUserById(nonExistingId);

            await act.Should().ThrowAsync<NotFoundException>();

            await _usersRepository.Received(1).GetById(nonExistingId);
        }

        [Fact]
        public async Task GetByEmail_WithExistedEmail_ShouldReturnUser()
        {
            string email = "john@mail.com";
            var expectedUser = UserEntity.Create(
                Guid.NewGuid(),
                "John Doe",
                email,
                "hash123",
                "photo_url",
                Enumerable.Empty<Guid>()
            ).User;

            _usersRepository.GetByEmail(email).Returns(expectedUser);

            var result = await _usersService.GetUserByEmail(email);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUser);

            await _usersRepository.Received(1).GetByEmail(email);
        }

        [Fact]
        public async Task GetByEmail_WithNonExistedEmail_ShouldReturnNull()
        {
            string email = "john@mail.com";
            
            _usersRepository.GetByEmail(email).Returns((UserEntity?)null);

            var result = await _usersService.GetUserByEmail(email);

            result.Should().BeNull();

            await _usersRepository.Received(1).GetByEmail(email);
        }
    }
}
