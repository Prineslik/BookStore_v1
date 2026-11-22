using BookStore.API.Mapping;
using BookStore.API.Middleware;
using BookStore.API.Requests.Book;
using BookStore.Application.Contracts.Books.Commands;
using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Interfaces.Users;
using BookStore.Application.Services;
using BookStore.Core.Entities;
using BookStore.Core.Exceptions;
using BookStore.Infrastructure.Repositories.Book;
using BookStore.Infrastructure.Repositories.User;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace BookStore.Tests.Integration.Middlewares
{
    public class GlobalExceptionMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private IServiceScope _scope;

        //private readonly ITestErrorStub _errorStub;
        //private readonly IBooksService _booksService;
        private readonly IUsersReadRepository _usersReadRepository;
        private readonly IBooksReadRepository _booksReadRepository;
        private readonly IBooksRepository _booksRepository;
        private readonly IBooksService _booksService;

        public static TheoryData<Func<Exception>, HttpStatusCode> ExceptionStatusCases =>
            new()
            {
                //{ () => new FluentValidation.ValidationException("Неверный формат данных"), HttpStatusCode.BadRequest},
                { () => new DuplicateException(""), HttpStatusCode.Conflict },
                { () => new DomainException(""), HttpStatusCode.InternalServerError},
                { () => new Exception(""), HttpStatusCode.InternalServerError}
            };

        public GlobalExceptionMiddlewareTests(WebApplicationFactory<Program> factory)
        {
            //_errorStub = Substitute.For<ITestErrorStub>();
            //_booksService = Substitute.For<IBooksService>();
            _usersReadRepository = Substitute.For<IUsersReadRepository>();
            _booksReadRepository = Substitute.For<IBooksReadRepository>();
            _booksRepository = Substitute.For<IBooksRepository>();
            _booksService = Substitute.For<IBooksService>();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    //services.RemoveAll<IBooksService>();
                    services.RemoveAll<IUsersReadRepository>();
                    services.RemoveAll<IBooksReadRepository>();
                    services.RemoveAll<IBooksRepository>();
                    services.RemoveAll<IBooksService>();
                    //services.RemoveAll<IUsersRepository>();

                    //services.AddSingleton(_booksService);
                    services.AddSingleton(_usersReadRepository);
                    services.AddSingleton(_booksReadRepository);
                    services.AddSingleton(_booksRepository);
                    services.AddSingleton(_booksService);
                    //services.AddSingleton(_usersRepository);

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

            //_scope = _factory.Services.CreateScope();
            //_booksService = _scope.ServiceProvider.GetRequiredService<IBooksService>();
        }

        [Theory]
        [MemberData(nameof(ExceptionStatusCases))]
        public async Task GetAsync_WhenEndpointThrowsException_ShouldReturnExpectedStatusCode(Func<Exception> exceptionFactory, HttpStatusCode statusCode)
        {
            var guid = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var guids = new List<Guid> { guid };

            var createResponse = new BookCreateRequest(
                "test book title",
                "test book description",
                3.99m,
                45,
                guids,
                guids);

            var exception = exceptionFactory();

            _usersReadRepository.HasPermission(guid, "Books.Create").Returns(true);
            _booksReadRepository.IsExistByTitle(Arg.Any<string>()).Returns(false);
            _booksService.CreateBook(Arg.Any<BookCreateCommand>()).ThrowsAsync(exception);

            using var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Test-User", guid.ToString());

            var response = await client.PostAsJsonAsync("/books/create/", createResponse);

            response.StatusCode.Should().Be(statusCode);
        }

        [Fact]
        public async Task GetAsync_WhenEndpointSucceeds_ShouldReturnOk()
        {
            var guid = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var client = _factory.CreateClient();
            var books = new List<BookShortDTO>() { };

            client.DefaultRequestHeaders.Add("X-Test-User", guid.ToString());

            _usersReadRepository.HasPermission(guid, "Books.Read").Returns(true);
            _booksService.GetAllBooks().Returns(Result<List<BookShortDTO>>.Success(books));

            var response = await client.GetAsync("/Books/all");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
