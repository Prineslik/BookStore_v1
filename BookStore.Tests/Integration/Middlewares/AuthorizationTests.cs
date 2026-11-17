using BookStore.Application.Authorization;
using BookStore.Application.Interfaces.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Net;

namespace BookStore.Tests.Integration.Middlewares
{
    public class AuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IUsersReadRepository _usersReadRepository = Substitute.For<IUsersReadRepository>();

        public AuthorizationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IUsersReadRepository>();
                    services.AddSingleton(_usersReadRepository);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                        options.DefaultForbidScheme = "Test";
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test",
                        _ => { });
                });
            });
        }

        [Fact]
        public async Task RequestToProtectedEndpoint_UserIsAuthAndHavePermission_ShouldReturnOk()
        {
            var userId = Guid.NewGuid();
            _usersReadRepository.HasPermission(userId, "books.read").Returns(Task.FromResult(true));
            using var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Add("X-Test-User", userId.ToString());

            var response = await client.GetAsync("/books/all");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await _usersReadRepository.Received(1).HasPermission(userId, "books.read");
        }

        [Fact]
        public async Task RequestToProtectedEndpoint_UserIsAuthAndNotHavePermission_ShouldReturnForbidden()
        {
            var userId = Guid.NewGuid();
            _usersReadRepository
                .HasPermission(userId, "books.read")
                .Returns(Task.FromResult(false));
            using var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Add("X-Test-User", userId.ToString());

            var response = await client.GetAsync("/books/all");
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.Forbidden);

            await _usersReadRepository
                .Received(1)
                .HasPermission(userId, "books.read");
        }

        [Fact]
        public async Task RequestToProtectedEndpoint_UserIsNotAuth_ShouldReturnUnauthorized()
        {
            using var client = _factory.CreateClient();

            var response = await client.GetAsync("/books/all");
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.Unauthorized);

            await _usersReadRepository
                .DidNotReceive()
                .HasPermission(Arg.Any<Guid>(), Arg.Any<string>());
        }

        [Fact]
        public async Task RequestToProtectedEndpoint_UserIsAuthWithInvalidId_ShouldReturnUnauthorized()
        {
            using var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Add("X-Test-User", "invalid id");

            var response = await client.GetAsync("/books/all");
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.Unauthorized);

            await _usersReadRepository
                .DidNotReceive()
                .HasPermission(Arg.Any<Guid>(), Arg.Any<string>());
        }

    }
}
