using BookStore.Application.DTOs;
using BookStore.Core.Entities;

namespace BookStore.Application.Interfaces
{
    public interface IBooksRepository
    {
        Task<Guid> Create(Guid id, string title, string description, decimal price);
        Task<Guid> Delete(Guid id);
        Task<List<BookEntity>> GetAll();
        Task<BookEntity?> GetById(Guid id);
        Task<List<BookEntity>> GetByTitle(string title);
        Task<PagedResult<BookEntity>> GetPagedAsync(ProductQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        Task<Guid> Update(Guid id, string title, string description, decimal price);
    }
}