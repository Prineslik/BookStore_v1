using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Core.Entities;

namespace BookStore.Application.Interfaces.Books
{
    public interface IBooksService
    {
        Task<Guid> CreateBook(BookEntity book);
        Task<Guid> DeleteBook(Guid id);
        Task<List<BookEntity>> GetAllBooks();
        Task<BookEntity?> GetBookById(Guid id);
        Task<List<BookEntity>> GetBooksByTitle(string title);
        Task<PagedResult<BookEntity>> GetPagedBookAsync(BookQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> UpdateBook(BookEntity book/*Guid id, string title, string description, decimal price*/);
    }
}