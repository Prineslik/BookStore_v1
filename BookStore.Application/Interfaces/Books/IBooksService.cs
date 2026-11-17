using BookStore.Application.Contracts.Books.Commands;
using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Books.Queries;
using BookStore.Application.Contracts.Common;
using BookStore.Core.Entities;

namespace BookStore.Application.Interfaces.Books
{
    public interface IBooksService
    {
        Task<Result<Guid>> CreateBook(BookCreateCommand createCommand);
        Task<Result<Guid>> UpdateBook(BookUpdateCommand updateCommand);
        Task<Result<Guid>> DeleteBook(Guid id);
        Task<Result<List<BookShortDTO>>> GetAllBooks();
        Task<Result<BookDetailsDTO?>> GetBookById(Guid id);
        Task<Result<List<BookShortDTO>>> GetBooksByTitle(string title);
        Task<Result<PagedResult<BookShortDTO>>> GetPagedBooks(BookQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
        //Task<Guid> ChangeBookOwners(Guid bookId, List<Guid> ownerIds);
    }
}