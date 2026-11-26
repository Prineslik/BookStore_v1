using BookStore.API.Requests.Book;
using BookStore.API.Requests.User;
using BookStore.Application.Authorization;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Roles;
using BookStore.Application.Interfaces.Users;
using BookStore.Application.Services;
using BookStore.Infrastructure.Repositories.User;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace BookStore.Tests.Integration.Middlewares
{
    public class AuthorizationTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private IServiceScope _scope;
        private IUsersReadRepository _usersReadRepository;

        public AuthorizationTests(IntegrationTestWebAppFactory factory)
        {
            _factory = factory;
            //_factory = factory.WithWebHostBuilder(builder =>
            //{
            //    builder.ConfigureTestServices(services =>
            //    {
            //        //services.RemoveAll<IUsersReadRepository>();
            //        //services.AddSingleton(_usersReadRepository);

            //        services.AddAuthentication(options =>
            //        {
            //            options.DefaultAuthenticateScheme = "Test";
            //            options.DefaultChallengeScheme = "Test";
            //            options.DefaultForbidScheme = "Test";
            //        })
            //        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
            //            "Test",
            //            _ => { });
            //    });
            //});
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
            _scope = _factory.Services.CreateScope();
            
            _usersReadRepository = _scope.ServiceProvider.GetRequiredService<IUsersReadRepository>();
        }

        public Task DisposeAsync()
        {
            _scope.Dispose();
            return Task.CompletedTask;
        }

        public static async Task AuthenticateAsAsync(HttpClient client, string email, string password)
        {
            var response = await client.PostAsJsonAsync("/Users/login", new LoginUserRequest(email, password));
            response.EnsureSuccessStatusCode();

            //var token = await client.//await response.Content.ReadAsStringAsync();
            //token = token.Trim('"');

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task RequestToProtectedEndpoint_UserIsAuthAndHavePermission_ShouldReturnOk()
        {
            using var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });

            await AuthenticateAsAsync(client, "testuser@example.com", "123");
            
            var response = await client.GetAsync("/users/me");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RequestToProtectedEndpoint_UserIsAuthAndNotHavePermission_ShouldReturnForbidden()
        {
            var guidList = new List<Guid> { Guid.Parse("11111111-1111-1111-1111-111111111111") };

            using var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("https://localhost")
                });

            await AuthenticateAsAsync(client, "testuser@example.com", "123");

            var response = await client.PostAsJsonAsync("/books/create/", new BookCreateRequest(
                "test book title",
                "test book description",
                3.99m,
                45,
                guidList,
                guidList));

            response.StatusCode
                .Should()
                .Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RequestToProtectedEndpoint_UserIsNotAuth_ShouldReturnUnauthorized()
        {
            using var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("https://localhost")
                });

            var response = await client.GetAsync("/users/me");
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RequestToUnprotectedEndpoint_UserIsNotAuth_ShouldReturnOk()
        {
            using var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("https://localhost")
                });

            var response = await client.GetAsync("/books/all");
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }

        //[Fact]
        //public async Task RequestToProtectedEndpoint_UserIsAuthWithInvalidId_ShouldReturnUnauthorized()
        //{
        //    using var client = _factory.CreateClient();

        //    client.DefaultRequestHeaders.Add("X-Test-User", "invalid id");

        //    var response = await client.GetAsync("/books/all");
        //    response.StatusCode
        //        .Should()
        //        .Be(HttpStatusCode.Unauthorized);

        //    //await _usersReadRepository
        //    //    .DidNotReceive()
        //    //    .HasPermission(Arg.Any<Guid>(), Arg.Any<string>());
        //}

    }
}
