using BookStore.Application.DTOs;
using BookStore.Core.Entities;

namespace BookStore.Application.Interfaces
{
    public interface IBooksService
    {
        Task<Guid> CreateBook(BookEntity book);
        Task<Guid> DeleteBook(Guid id);
        Task<List<BookEntity>> GetAllBooks();
        Task<BookEntity?> GetBookById(Guid id);
        Task<List<BookEntity>> GetBooksByTitle(string title);
        Task<PagedResult<BookEntity>> GetPagedBookAsync(ProductQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> UpdateBook(Guid id, string title, string description, decimal price);
    }
}