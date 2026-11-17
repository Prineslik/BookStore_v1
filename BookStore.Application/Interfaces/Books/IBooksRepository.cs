using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Common;
using BookStore.Core.Entities;

namespace BookStore.Application.Interfaces.Books
{
    public interface IBooksRepository
    {
        Task<Guid> Create(BookEntity newBookEntity);
        Task<Guid> Delete(BookEntity bookEntity);
        Task<Guid> Update(BookEntity updatedBookEntity);
        Task<BookEntity?> GetById(Guid id);
    }
}