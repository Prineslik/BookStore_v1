using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Books.Queries;
using BookStore.Application.Contracts.Common;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Interfaces.Books
{
    public interface IBooksReadRepository
    {
        Task<List<BookShortDTO>> GetAll();
        Task<BookDetailsDTO?> GetById(Guid id);
        Task<List<BookShortDTO>> GetByTitle(string title);
        Task<BookDetailsDTO?> GetByTitleStrict(string title);
        Task<bool> IsExistById(Guid id);
        Task<bool> IsExistByTitle(string title);
        Task<PagedResult<BookShortDTO>> GetPaged(BookQueryParameters parameters/*, CancellationToken cancellationToken = default*/);
    }
}
