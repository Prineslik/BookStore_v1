using BookStore.Application.Contracts.Authors.DTO;
using BookStore.Application.Contracts.Books.Commands;
using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Books.Queries;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users.DTO;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Interfaces.Users;
using BookStore.Application.Services;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Repositories.Book;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace BookStore.Tests.Unit.Application.Services
{
    public class BooksServiceTests
    {
        private readonly IBooksRepository _booksRepository = Substitute.For<IBooksRepository>();
        private readonly IBooksReadRepository _booksReadRepository = Substitute.For<IBooksReadRepository>();
        private readonly IUsersReadRepository _usersReadRepository = Substitute.For<IUsersReadRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        
        private readonly BooksService _booksService;

        public BooksServiceTests()
        {
            _booksService = new BooksService(_booksRepository, _booksReadRepository, _usersReadRepository, _unitOfWork);
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnSuccess()
        {
            var books = new List<BookShortDTO>
            {
                new BookShortDTO(Guid.NewGuid(), "book 1", "book description", 2.55m, 3, DateTime.UtcNow, null),
                new BookShortDTO(Guid.NewGuid(), "book 2", "book description", 3.66m, 6, DateTime.UtcNow, DateTime.UtcNow)
            };

            var expectedResult = Result<List<BookShortDTO>>.Success(books);

            _booksReadRepository.GetAll().Returns(books);

            var result = await _booksService.GetAllBooks();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            await _booksReadRepository.Received(1).GetAll();
        }

        [Fact]
        public async Task GetBooksById_WhenBookExists_ShouldReturnSuccess()
        {
            var bookId = Guid.NewGuid();
            var owners = new List<OwnerShortDTO> { new OwnerShortDTO(Guid.NewGuid(), "owner 1", "owner1@mail.ru", "https://picture.jpg", false) };
            var authors = new List<AuthorShortDTO> { new AuthorShortDTO(Guid.NewGuid(), "author 1", "https://picture.jpg") };

            var book = new BookDetailsDTO(
                bookId,
                "book",
                "desc",
                5.78m,
                4,
                DateTime.Now,
                null,
                owners,
                authors);

            var expectedResult = Result<BookDetailsDTO>.Success(book);

            _booksReadRepository.GetById(bookId).Returns(book);

            var result = await _booksService.GetBookById(bookId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            await _booksReadRepository.Received(1).GetById(bookId);
        }

        [Fact]
        public async Task GetBookById_WhenBookDoesNotExist_ShouldReturnFailure()
        {
            var nonExistingId = Guid.NewGuid();
            BookDetailsDTO? book = null;
            var expectedResult = Result<BookDetailsDTO>.Failure(Error.NotFound("Book", "Id", nonExistingId));

            _booksReadRepository.GetById(nonExistingId).Returns(book);

            var result = await _booksService.GetBookById(nonExistingId);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(expectedResult.Error);

            await _booksReadRepository.Received(1).GetById(nonExistingId);
        }

        [Fact]
        public async Task GetBooksByTitle_WithExistedTitle_ShouldReturnSuccess()
        {
            string title = "book title";

            var books = new List<BookShortDTO>
            {
                new BookShortDTO(Guid.NewGuid(), "book title 1", "book description", 2.55m, 3, DateTime.UtcNow, null),
                new BookShortDTO(Guid.NewGuid(), "book title 2", "book description", 3.66m, 6, DateTime.UtcNow, DateTime.UtcNow)
            };

            var expectedResult = Result<List<BookShortDTO>>.Success(books);

            _booksReadRepository.GetByTitle(title).Returns(books);

            var result = await _booksService.GetBooksByTitle(title);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            await _booksReadRepository.Received(1).GetByTitle(title);
        }

        [Fact]
        public async Task GetBooksByTitle_WithNonExistedTitle_ShouldReturnSuccess()
        {
            string title = "book title";
            List<BookShortDTO> books = new List<BookShortDTO>();
            var expectedResult = Result<List<BookShortDTO>>.Success(books);

            _booksReadRepository.GetByTitle(title).Returns(books);

            var result = await _booksService.GetBooksByTitle(title);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            await _booksReadRepository.Received(1).GetByTitle(title);
        }

        [Fact]
        public async Task CreateBook_WithValidBookEntity_ShouldReturnSuccess()
        {
            var owners = new List<Guid> { Guid.NewGuid() };
            var authors = new List<Guid> { Guid.NewGuid() };

            var createBookCommand = new BookCreateCommand("title", "description", 4.32m, 56, owners, authors);
            var bookId = Guid.NewGuid();

            var expectedResult = Result<Guid>.Success(bookId);

            _booksReadRepository.IsExistByTitle(createBookCommand.Title).Returns(false);
            _booksRepository.Create(Arg.Any<BookEntity>()).Returns(bookId);

            var result = await _booksService.CreateBook(createBookCommand);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            await _booksRepository.Received(1).Create(Arg.Any<BookEntity>());
            await _booksReadRepository.Received(1).IsExistByTitle(createBookCommand.Title);
            await _unitOfWork.Received(1).CommitAsync();
        }

        [Fact]
        public async Task CreateBook_WithDuplicatedTitle_ShouldReturnFailure()
        {
            var owners = new List<Guid> { Guid.NewGuid() };
            var authors = new List<Guid> { Guid.NewGuid() };

            var createBookCommand = new BookCreateCommand("title", "description", 4.32m, 56, owners, authors);
            
            var expectedResult = Result<Guid>.Failure(Error.Duplicate("Title", createBookCommand.Title));

            _booksReadRepository.IsExistByTitle(Arg.Any<string>()).Returns(true);

            var result = await _booksService.CreateBook(createBookCommand);

            result.Should().NotBeNull();
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(expectedResult.Error);

            await _booksReadRepository.Received(1).IsExistByTitle(Arg.Any<string>());
            await _booksRepository.DidNotReceive().Create(Arg.Any<BookEntity>());
            await _unitOfWork.DidNotReceive().CommitAsync();
        }

        [Fact]
        public async Task CreateBook_WhenDbUpdateException_ShouldThrowDbUpdateException()
        {
            var owners = new List<Guid> { Guid.NewGuid() };
            var authors = new List<Guid> { Guid.NewGuid() };

            var createBookCommand = new BookCreateCommand("title", "description", 4.32m, 56, owners, authors);
            
            _booksReadRepository.IsExistByTitle(createBookCommand.Title).Returns(false);

            _booksRepository.Create(Arg.Any<BookEntity>()).ThrowsAsync(new DbUpdateException());

            Func<Task> act = async () => await _booksService.CreateBook(createBookCommand);

            await act.Should().ThrowAsync<DbUpdateException>();

            await _booksRepository.Received(1).Create(Arg.Any<BookEntity>());
            await _unitOfWork.DidNotReceive().CommitAsync();
        }

        [Fact]
        public async Task UpdateBook_WithValidBookEntity_ShouldReturnSuccess()
        {
            var ownerIds = new List<Guid> { Guid.NewGuid() };
            var authorIds = new List<Guid> { Guid.NewGuid() };
            var owners = new List<OwnerShortDTO> { new OwnerShortDTO(
                Guid.NewGuid(),
                "name",
                "email",
                "url",
                false) };
            var authors = new List<AuthorShortDTO> { new AuthorShortDTO(
                Guid.NewGuid(),
                "name",
                "url") };

            var updateBookCommand = new BookUpdateCommand(Guid.NewGuid(), "title", "description", 4.32m, 56, ownerIds, authorIds);

            BookDetailsDTO? bookDTO = null;

            var bookEntity = BookEntity.Reconstitute(
                updateBookCommand.Id,
                updateBookCommand.Title,
                updateBookCommand.Description,
                updateBookCommand.Price,
                updateBookCommand.StockQuantity,
                DateTime.UtcNow,
                null,
                ownerIds,
                authorIds);

            var expectedResult = Result<Guid>.Success(bookEntity.Id);

            _booksReadRepository.IsExistById(updateBookCommand.Id).Returns(true);
            _booksReadRepository.GetByTitleStrict(updateBookCommand.Title).Returns(bookDTO);
            _usersReadRepository.IsExistById(Arg.Any<Guid>()).Returns(true);
            _booksRepository.GetById(updateBookCommand.Id).Returns(bookEntity);

            _booksRepository.Update(bookEntity).Returns(bookEntity.Id);

            var result = await _booksService.UpdateBook(updateBookCommand);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            await _booksRepository.Received(1).Update(bookEntity);
            await _booksReadRepository.Received(1).IsExistById(updateBookCommand.Id);
            await _booksReadRepository.Received(1).GetByTitleStrict(updateBookCommand.Title);
            await _usersReadRepository.Received(updateBookCommand.OwnerIds.Count).IsExistById(Arg.Any<Guid>());
            await _booksRepository.Received(1).GetById(updateBookCommand.Id);
            await _unitOfWork.Received(1).CommitAsync();
        }

        [Fact]
        public async Task UpdateBook_WhenDbUpdateConcurrencyException_ShouldThrowDbConcurrencyException()
        {
            var ownerIds = new List<Guid> { Guid.NewGuid() };
            var authorIds = new List<Guid> { Guid.NewGuid() };
            var owners = new List<OwnerShortDTO> { new OwnerShortDTO(
                Guid.NewGuid(),
                "name",
                "email",
                "url",
                false) };
            var authors = new List<AuthorShortDTO> { new AuthorShortDTO(
                Guid.NewGuid(),
                "name",
                "url") };

            var updateBookCommand = new BookUpdateCommand(Guid.NewGuid(), "title", "description", 4.32m, 56, ownerIds, authorIds);

            BookDetailsDTO? bookDTO = null;

            var bookEntity = BookEntity.Reconstitute(
                updateBookCommand.Id,
                updateBookCommand.Title,
                updateBookCommand.Description,
                updateBookCommand.Price,
                updateBookCommand.StockQuantity,
                DateTime.UtcNow,
                null,
                ownerIds,
                authorIds);

            _booksReadRepository.IsExistById(updateBookCommand.Id).Returns(true);
            _booksReadRepository.GetByTitleStrict(updateBookCommand.Title).Returns(bookDTO);
            _usersReadRepository.IsExistById(Arg.Any<Guid>()).Returns(true);
            _booksRepository.GetById(updateBookCommand.Id).Returns(bookEntity);

            _booksRepository.Update(bookEntity).ThrowsAsync(new DbUpdateConcurrencyException());

            Func<Task> act = async () => await _booksService.UpdateBook(updateBookCommand);

            await act.Should().ThrowAsync<DbUpdateConcurrencyException>();

            await _booksRepository.Received(1).Update(bookEntity);
            await _booksReadRepository.Received(1).IsExistById(updateBookCommand.Id);
            await _booksReadRepository.Received(1).GetByTitleStrict(updateBookCommand.Title);
            await _usersReadRepository.Received(updateBookCommand.OwnerIds.Count).IsExistById(Arg.Any<Guid>());
            await _booksRepository.Received(1).GetById(updateBookCommand.Id);
            await _unitOfWork.DidNotReceive().CommitAsync();
        }

        [Fact]
        public async Task DeleteBook_WithExistedId_ShouldSuccess()
        {
            Guid bookId = Guid.NewGuid();
            var owners = new List<Guid> { Guid.NewGuid() };
            var authors = new List<Guid> { Guid.NewGuid() };

            var book = BookEntity.Reconstitute(bookId, "title", "description", 4.32m, 56, DateTime.UtcNow, null, owners, authors);

            _booksRepository.GetById(bookId).Returns(book);
            _booksRepository.Delete(book).Returns(bookId);

            var expectedResult = Result<Guid>.Success(bookId);

            var result = await _booksService.DeleteBook(bookId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            await _booksRepository.Received(1).GetById(bookId);
            await _booksRepository.Received(1).Delete(book);
            await _unitOfWork.Received(1).CommitAsync();
        }

        [Fact]
        public async Task GetPagedBooksAsync_WithValidQueryParameters_ShouldReturnSuccess()
        {
            var bookQueryParameters = new BookQueryParameters() { SearchTerm = "ook" };

            var books = new List<BookShortDTO>
            {
                new BookShortDTO(Guid.NewGuid(), "Book 1", "desc 1", 4.55m, 78, DateTime.UtcNow, null),
                new BookShortDTO(Guid.NewGuid(), "Book 2", "desc 2", 4.55m, 78, DateTime.UtcNow, null)
            };

            var pagedResult = new PagedResult<BookShortDTO>()
            {
                Items = books,
                TotalCount = books.Count(),
                PageNumber = bookQueryParameters.PageNumber,
                PageSize = bookQueryParameters.PageSize,

            };

            var expectedResult = Result<PagedResult<BookShortDTO>>.Success(pagedResult);

            _booksReadRepository.GetPaged(bookQueryParameters).Returns(pagedResult);

            var result = await _booksService.GetPagedBooks(bookQueryParameters);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            await _booksReadRepository.Received(1).GetPaged(bookQueryParameters);
        }
    }
}
