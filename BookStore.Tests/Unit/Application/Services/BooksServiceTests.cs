using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Services;
using BookStore.Core.Entities;
using FluentAssertions;
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

        private readonly BooksService _booksService;

        public BooksServiceTests()
        {
            _booksService = new BooksService(_booksRepository);
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnBooks()
        {
            var books = new List<BookEntity?>
            {
                BookEntity.Create(Guid.NewGuid(), "book 1", "book description", 2.55m).Book,
                BookEntity.Create(Guid.NewGuid(), "book 2", "book description", 3.66m).Book
            };

            _booksRepository.GetAll().Returns(books);

            var result = await _booksService.GetAllBooks();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(books);

            await _booksRepository.Received(1).GetAll();
        }

        [Fact]
        public async Task GetBooksById_WhenBookExists_ShouldReturnBook()
        {
            var bookId = Guid.NewGuid();
            var expectedBook = BookEntity.Create(
                bookId,
                "book",
                "desc",
                5.78m
            ).Book;

            _booksRepository.GetById(bookId).Returns(expectedBook);

            var result = await _booksService.GetBookById(bookId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedBook);

            await _booksRepository.Received(1).GetById(bookId);
        }

        [Fact]
        public async Task GetBookById_WhenBookDoesNotExist_ShouldThrowNotFoundException()
        {
            var nonExistingId = Guid.NewGuid();

            _booksRepository.GetById(nonExistingId).ThrowsAsync(new NotFoundException("Book", nonExistingId));

            Func<Task> act = async () => await _booksService.GetBookById(nonExistingId);

            await act.Should().ThrowAsync<NotFoundException>();

            await _booksRepository.Received(1).GetById(nonExistingId);
        }

        [Fact]
        public async Task GetBooksByTitle_WithExistedTitle_ShouldReturnBook()
        {
            string title = "book title";
            var expectedBooks = new List<BookEntity>
            {
                BookEntity.Create(Guid.NewGuid(), "book 1", "book description", 2.55m).Book,
                BookEntity.Create(Guid.NewGuid(), "book 2", "book description", 3.66m).Book
            };

            _booksRepository.GetByTitle(title).Returns(expectedBooks);

            var result = await _booksService.GetBooksByTitle(title);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedBooks);

            await _booksRepository.Received(1).GetByTitle(title);
        }

        [Fact]
        public async Task GetBooksByTitle_WithNonExistedTitle_ShouldReturnNull()
        {
            string title = "book title";
            List<BookEntity> expectedBooks = new List<BookEntity>();

            _booksRepository.GetByTitle(title).Returns(expectedBooks);

            var result = await _booksService.GetBooksByTitle(title);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedBooks);

            await _booksRepository.Received(1).GetByTitle(title);
        }

        [Fact]
        public async Task CreateBook_WithValidBookEntity_ShouldReturnBookId()
        {
            var title = "book";
            var description = "desc";
            var price = 4.99m;

            var book = BookEntity.Create(Guid.NewGuid(), title, description, price).Book;

            _booksRepository.Create(book).Returns(book.Id);

            var result = await _booksService.CreateBook(book);

            result.Should()
                .NotBeEmpty()
                .And.Be(book.Id);
        }

        [Fact]
        public async Task CreateBook_WithDuplicatedId_ShouldThrowDuplicatedException()
        {
            var title = "book";
            var description = "desc";
            var price = 4.99m;

            var book = BookEntity.Create(Guid.NewGuid(), title, description, price).Book;

            _booksRepository.Create(book).ThrowsAsync(new DuplicateException(""));

            Func<Task> act = async () => await _booksService.CreateBook(book);

            await act.Should().ThrowAsync<DuplicateException>();

            await _booksRepository.Received(1).Create(book);
        }

        [Fact]
        public async Task CreateBook_WhenDbUpdateException_ShouldThrowInfrastructureException()
        {
            var title = "book";
            var description = "desc";
            var price = 4.99m;

            var book = BookEntity.Create(Guid.NewGuid(), title, description, price).Book;

            _booksRepository.Create(book).ThrowsAsync(new InfrastructureException(""));

            Func<Task> act = async () => await _booksService.CreateBook(book);

            await act.Should().ThrowAsync<InfrastructureException>();

            await _booksRepository.Received(1).Create(book);
        }

        [Fact]
        public async Task UpdateBook_WithValidBookEntity_ShouldReturnBookId()
        {
            var title = "book";
            var description = "desc";
            var price = 4.99m;

            var book = BookEntity.Create(Guid.NewGuid(), title, description, price).Book;

            _booksRepository.Update(book).Returns(book.Id);

            var result = await _booksService.UpdateBook(book);

            result.Should()
                .NotBeEmpty()
                .And.Be(book.Id);

            await _booksRepository.Received(1).Update(book);
        }

        [Fact]
        public async Task UpdateBook_WhenDbUpdateConcurrencyException_ShouldThrowDbConcurrencyException()
        {
            var title = "book";
            var description = "desc";
            var price = 4.99m;

            var book = BookEntity.Create(Guid.NewGuid(), title, description, price).Book;

            _booksRepository.Update(book).ThrowsAsync(new DbConcurrencyException(""));

            Func<Task> act = async () => await _booksService.UpdateBook(book);

            await act.Should().ThrowAsync<DbConcurrencyException>();

            await _booksRepository.Received(1).Update(book);
        }

        [Fact]
        public async Task DeleteBook_WithExistedId_ShouldReturnBookId()
        {
            Guid bookId = Guid.NewGuid();

            _booksRepository.Delete(bookId).Returns(bookId);

            var result = await _booksService.DeleteBook(bookId);

            result.Should().Be(bookId);
        }

        [Fact]
        public async Task DeleteBook_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            Guid bookId = Guid.NewGuid();

            _booksRepository.Delete(bookId).ThrowsAsync(new InfrastructureException($"Failed to delete book with GUID = {bookId}"));

            Func<Task> act = async () => await _booksService.DeleteBook(bookId);

            await act.Should().ThrowAsync<InfrastructureException>();
            //result.Should().Be(book.Id);
        }

        [Fact]
        public async Task GetPagedBooksAsync_WithValidQueryParameters_ShuoldReturnPagedResult()
        {
            var bookQueryParameters = new BookQueryParameters() { SearchTerm = "ook" };

            var expectedBooks = new List<BookEntity>
            {
                BookEntity.Create(Guid.NewGuid(), "Book 1", "desc 1", 4.55m).Book,
                BookEntity.Create(Guid.NewGuid(), "Book 2", "desc 2", 4.55m).Book
            };

            var expectedPagedResult = new PagedResult<BookEntity>()
            {
                Items = expectedBooks,
                TotalCount = expectedBooks.Count(),
                PageNumber = bookQueryParameters.PageNumber,
                PageSize = bookQueryParameters.PageSize,

            };

            _booksRepository.GetPaged(bookQueryParameters).Returns(expectedPagedResult);

            var result = await _booksService.GetPagedBooks(bookQueryParameters);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedPagedResult);

            await _booksRepository.Received(1).GetPaged(Arg.Any<BookQueryParameters>());
        }
    }
}
