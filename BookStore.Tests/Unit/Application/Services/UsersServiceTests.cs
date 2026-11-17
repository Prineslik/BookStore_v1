using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users.Commands;
using BookStore.Application.Contracts.Users.Queries;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BookStore.Tests.Unit.Application.Services
{
    public class UsersServiceTests
    {
        private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
        private readonly IUsersReadRepository _usersReadRepository = Substitute.For<IUsersReadRepository>();
        private readonly IRolesReadRepository _rolesReadRepository = Substitute.For<IRolesReadRepository>();
        private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
        private readonly IJwtProvider _jwtProvider = Substitute.For<IJwtProvider>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        private readonly UsersService _sut;

        public UsersServiceTests()
        {
            _sut = new UsersService(_usersRepository, _usersReadRepository, _passwordHasher, _jwtProvider, _rolesReadRepository, _unitOfWork);
        }

        [Fact]
        public async Task CreateUser_WhenDataIsValid_ShouldCreateNewUser()
        {
            var registerUserCommand = new RegisterUserCommand(
                "new user",
                "new@mail.ru",
                "465");

            string hashedPassword = "hashed_password";

            var defaultRole = Guid.NewGuid();

            var expectedId = Guid.NewGuid();

            _usersReadRepository.IsExistByEmail(registerUserCommand.Email).Returns(false);
            _passwordHasher.Hash(registerUserCommand.Password).Returns(hashedPassword);
            _rolesReadRepository.GetDefaultRole().Returns(defaultRole);
            _usersRepository.Create(Arg.Any<UserEntity>()).Returns(expectedId);

            var result = await _sut.CreateUser(registerUserCommand);

            result.Value.Should().Be(expectedId);

            await _usersReadRepository.Received(1).IsExistByEmail(Arg.Any<string>());
            _passwordHasher.Received(1).Hash(Arg.Any<string>());
            await _rolesReadRepository.Received(1).GetDefaultRole();
            await _usersRepository.Received(1).Create(
                Arg.Is<UserEntity>(user =>
                    user.Email == registerUserCommand.Email &&
                    user.UserName == registerUserCommand.Name &&
                    user.PasswordHash == hashedPassword &&
                    user.RoleIds.Contains(defaultRole)));
            await _unitOfWork.Received(1).CommitAsync();
        }

        [Fact]
        public async Task CreateUser_WhenEmailAlreadyExists_ShouldReturnFailure()
        {
            var registerUserCommand = new RegisterUserCommand(
                "new user",
                "new@mail.ru",
                "465");

            var expectedError = Error.Duplicate("Email", registerUserCommand.Email);

            _usersReadRepository.IsExistByEmail(registerUserCommand.Email).Returns(true);

            var result = await _sut.CreateUser(registerUserCommand);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(expectedError);

            await _usersReadRepository.Received(1).IsExistByEmail(Arg.Any<string>());

            _passwordHasher.DidNotReceive().Hash(Arg.Any<string>());
            await _rolesReadRepository.DidNotReceive().GetDefaultRole();
            await _usersRepository.DidNotReceive().Create(Arg.Any<UserEntity>());
            await _unitOfWork.DidNotReceive().CommitAsync();
        }

        [Fact]
        public async Task CreateUser_WhenRepositoryCreateFails_ShouldNotCommit()
        {
            var registerUserCommand = new RegisterUserCommand(
                "new user",
                "new@mail.ru",
                "465");

            string hashedPassword = "hashed_password";

            var defaultRole = Guid.NewGuid();

            _usersReadRepository.IsExistByEmail(registerUserCommand.Email).Returns(false);
            _passwordHasher.Hash(registerUserCommand.Password).Returns(hashedPassword);
            _rolesReadRepository.GetDefaultRole().Returns(defaultRole);
            _usersRepository.Create(Arg.Any<UserEntity>()).Returns<Task<Guid>>(_ => throw new InvalidOperationException());

            Func<Task> act = async () => await _sut.CreateUser(registerUserCommand);

            await act.Should().ThrowAsync<InvalidOperationException>();

            await _usersReadRepository.Received(1).IsExistByEmail(Arg.Any<string>());
            _passwordHasher.Received(1).Hash(Arg.Any<string>());
            await _rolesReadRepository.Received(1).GetDefaultRole();
            await _usersRepository.Received(1).Create(
                Arg.Is<UserEntity>(user =>
                    user.Email == registerUserCommand.Email &&
                    user.UserName == registerUserCommand.Name &&
                    user.PasswordHash == hashedPassword &&
                    user.RoleIds.Contains(defaultRole)));
            await _unitOfWork.DidNotReceive().CommitAsync();
        }

        [Fact]
        public async Task CreateUser_WhenUoWCreateFail_ShouldThrowException()
        {
            var registerUserCommand = new RegisterUserCommand(
                "new user",
                "new@mail.ru",
                "465");

            string hashedPassword = "hashed_password";

            var defaultRole = Guid.NewGuid();

            _usersReadRepository.IsExistByEmail(registerUserCommand.Email).Returns(false);
            _passwordHasher.Hash(registerUserCommand.Password).Returns(hashedPassword);
            _rolesReadRepository.GetDefaultRole().Returns(defaultRole);
            _usersRepository.Create(Arg.Any<UserEntity>()).Returns<Guid>(Guid.NewGuid());
            _unitOfWork.CommitAsync().ThrowsAsync<InvalidOperationException>();

            Func<Task> act = async () => await _sut.CreateUser(registerUserCommand);

            await act.Should().ThrowAsync<InvalidOperationException>();

            await _usersReadRepository.Received(1).IsExistByEmail(Arg.Any<string>());
            _passwordHasher.Received(1).Hash(Arg.Any<string>());
            await _rolesReadRepository.Received(1).GetDefaultRole();
            await _usersRepository.Received(1).Create(
                Arg.Is<UserEntity>(user =>
                    user.Email == registerUserCommand.Email &&
                    user.UserName == registerUserCommand.Name &&
                    user.PasswordHash == hashedPassword &&
                    user.RoleIds.Contains(defaultRole)));
            await _unitOfWork.Received(1).CommitAsync();
        }
    }
}
