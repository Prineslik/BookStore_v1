using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Core.Entities;

namespace BookStore.Application.Interfaces.Books
{
    public interface IBooksRepository
    {
        Task<Guid> Create(BookEntity newBookEntity/*Guid id, string title, string description, decimal price*/);
        Task<Guid> Delete(Guid id);
        Task<List<BookEntity>> GetAll();
        Task<BookEntity?> GetById(Guid id);
        Task<List<BookEntity>> GetByTitle(string title);
        Task<bool> IsExist(Guid id);
        Task<PagedResult<BooksResponse>> GetPaged(BookQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> Update(BookEntity updatedBookEntity/*Guid id, string title, string description, decimal price*/);
    }
}