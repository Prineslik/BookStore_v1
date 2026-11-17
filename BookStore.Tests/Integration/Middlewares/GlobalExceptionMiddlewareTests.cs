using BookStore.API.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;
using NSubstitute;
using NSubstitute.Extensions;
using NSubstitute.ClearExtensions;
using FluentAssertions;
using BookStore.Application.Exceptions;
using BookStore.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BookStore.Application.Interfaces.Users;

namespace BookStore.Tests.Integration.Middlewares
{
    public class GlobalExceptionMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestErrorStub _errorStub;
        private readonly IUsersReadRepository _usersReadRepository;

        public static TheoryData<Func<Exception>, HttpStatusCode> ExceptionStatusCases =>
            new()
            {
                { () => new FluentValidation.ValidationException("Неверный формат данных"), HttpStatusCode.BadRequest},
                { () => new DuplicateException(""), HttpStatusCode.Conflict },
                { () => new DomainException(""), HttpStatusCode.InternalServerError},
                { () => new Exception(""), HttpStatusCode.InternalServerError}
            };

        public GlobalExceptionMiddlewareTests(WebApplicationFactory<Program> factory)
        {
            _errorStub = Substitute.For<ITestErrorStub>();
            _usersReadRepository = Substitute.For<IUsersReadRepository>();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IUsersReadRepository>();
                    services.AddSingleton(_usersReadRepository);

                    services.AddSingleton(_errorStub);
                });

                //builder.Configure(app =>
                //{
                //    app.UseMiddleware<GlobalExceptionMiddleware>();

                //    app.UseRouting();
                //    app.UseEndpoints(endpoints =>
                //    {
                //        endpoints.MapGet("/test-error", (ITestErrorStub stub) =>
                //        {
                //            stub.ThrowIfConfigured();
                //            return Results.Ok();
                //        });
                //        endpoints.MapGet("/test-ok", () =>
                //        {
                //            return Results.Ok();
                //        });
                //    });
                //});
            });
        }

        [Theory]
        [MemberData(nameof(ExceptionStatusCases))]
        public async Task GetAsync_WhenEndpointThrowsException_ShouldReturnExpectedStatusCode(Func<Exception> exceptionFactory, HttpStatusCode statusCode)
        {
            var exception = exceptionFactory();

            _errorStub
                .When(x => x.ThrowIfConfigured())
                .Do(x => throw exception);

            var client = _factory.CreateClient();
            
            var response = await client.GetAsync("/books/create");
            response.StatusCode.Should().Be(statusCode);
        }

        [Fact]
        public async Task GetAsync_WhenEndpointSucceeds_ShouldReturnOk()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/Books/all");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
