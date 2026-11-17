using BookStore.Application.Contracts.Users.Commands;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Roles;
using BookStore.Application.Interfaces.Users;
using BookStore.Core.Entities;
using BookStore.Core.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BookStore.Tests.Integration.Services
{
    public class UsersServiceTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
    {
        private IntegrationTestWebAppFactory _factory;
        private IServiceScope _scope;
        private IUsersRepository _usersRepository;
        private IUsersReadRepository _usersReadRepository;
        private IUsersService _usersService;

        private IRolesReadRepository _rolesReadRepository;
        private IPasswordHasher _passwordHasher;
        private IJwtProvider _jwtProvider;
        private IUnitOfWork _unitOfWork;

        public static TheoryData<RegisterUserCommand, string> GetInvalidRegisterUserCommands =>
            new()
            {
                { new RegisterUserCommand("","vaild@mail.ru", "vaildpassword123"),"Name can not be empty"},
                { new RegisterUserCommand(new string('s',UserEntity.MAX_NAME_LENGTH + 1),"vaild@mail.ru", "vaildpassword123"),$"Name can not be longer than {UserEntity.MAX_NAME_LENGTH}"},
                { new RegisterUserCommand("name","invaild_mail.ru", "vaildpassword123"),"Email is incorrect"}
            };

        public UsersServiceTests(IntegrationTestWebAppFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            
            await _factory.ResetDatabaseAsync();
            _scope = _factory.Services.CreateScope();
            _usersRepository = _scope.ServiceProvider.GetRequiredService<IUsersRepository>();
            _usersReadRepository = _scope.ServiceProvider.GetRequiredService<IUsersReadRepository>();
            _usersService = _scope.ServiceProvider.GetRequiredService<IUsersService>();
            _rolesReadRepository = _scope.ServiceProvider.GetRequiredService<IRolesReadRepository>();
            _passwordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            _jwtProvider = _scope.ServiceProvider.GetRequiredService<IJwtProvider>();
            _unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        }

        public Task DisposeAsync()
        {
            _scope.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Create_WhenDataIsValid_ShouldReturnSuccess()
        {
            var registerUserCommand = new RegisterUserCommand(
                "new user",
                "new@mail.ru",
                "465");

            var createResult = await _usersService.CreateUser(registerUserCommand);

            var newUser = await _usersRepository.GetById(createResult.Value);

            createResult.Value.Should().NotBeEmpty();

            newUser.Should().Match<UserEntity>(u =>
                u.UserName == registerUserCommand.Name &&
                u.Email == registerUserCommand.Email &&
                _passwordHasher.Verify(registerUserCommand.Password, u.PasswordHash));
        }

        [Theory]
        [MemberData(nameof(GetInvalidRegisterUserCommands))]
        public async Task Create_WhenDataIsInvalid_ShouldThrowDomainException(RegisterUserCommand registerUserCommand, string exceptionMessage)
        {
            Func<Task> act = async () => await _usersService.CreateUser(registerUserCommand);

            await act.Should().ThrowAsync<DomainException>().WithMessage(exceptionMessage);
        }
    }
}
