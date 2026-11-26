using BookStore.Application.Contracts.Books.Commands;
using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Books.Queries;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Books;
using BookStore.Application.Interfaces.Users;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Services
{
    public class BooksService : IBooksService
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IBooksReadRepository _booksReadRepository;
        private readonly IUsersReadRepository _usersReadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BooksService(IBooksRepository booksRepository, IBooksReadRepository booksReadRepository, IUsersReadRepository usersReadRepository, IUnitOfWork unitOfWork)
        {
            _booksRepository = booksRepository;
            _booksReadRepository = booksReadRepository;
            _usersReadRepository = usersReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<BookShortDTO>>> GetAllBooks()
        {
            var books = await _booksReadRepository.GetAll();
            return Result<List<BookShortDTO>>.Success(books);
        }

        public async Task<Result<BookDetailsDTO?>> GetBookById(Guid id)
        {
            var book = await _booksReadRepository.GetById(id);
            if (book == null)
                return Result<BookDetailsDTO?>.Failure(Error.NotFound("Book", "Id", id));

            return Result<BookDetailsDTO?>.Success(book);
        }

        public async Task<Result<List<BookShortDTO>>> GetBooksByTitle(string title)
        {
            var book = await _booksReadRepository.GetByTitle(title);
            return Result<List<BookShortDTO>>.Success(book);
        }

        public async Task<Result<Guid>> CreateBook(BookCreateCommand createCommand)
        {
            if (await _booksReadRepository.IsExistByTitle(createCommand.Title))
                return Result<Guid>.Failure(Error.Duplicate("Title", createCommand.Title));

            var bookEntity = BookEntity.Create(
                createCommand.Title,
                createCommand.Description,
                createCommand.Price,
                createCommand.StockQuantity,
                createCommand.OwnerIds,
                createCommand.AuthorIds);

            var createdBookId = await _booksRepository.Create(bookEntity);

            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(createdBookId);
        }

        public async Task<Result<Guid>> UpdateBook(BookUpdateCommand updateCommand)
        {
            if (!await _booksReadRepository.IsExistById(updateCommand.Id))
                return Result<Guid>.Failure(Error.NotFound("Book", "Id", updateCommand.Id));

            var duplicate = await _booksReadRepository.GetByTitleStrict(updateCommand.Title);
            if (duplicate != null && duplicate.Id != updateCommand.Id)
                return Result<Guid>.Failure(Error.Duplicate("Title", updateCommand.Title));

            foreach (var ownerId in updateCommand.OwnerIds)
                if (!await _usersReadRepository.IsExistById(ownerId))
                    return Result<Guid>.Failure(Error.NotFound("User", "Id", ownerId));

            //добавить сервис для автора

            //foreach (var authorId in updateCommand.AuthorIds)
            //    if (!await _authorRepository.IsExist(authorId))
            //        throw new NotFoundException("Author", authorId);

            var book = await _booksRepository.GetById(updateCommand.Id);

            book.Rename(updateCommand.Title);
            book.ChangeDescription(updateCommand.Description);
            book.ChangePrice(updateCommand.Price);
            book.ChangeStockQuantity(updateCommand.StockQuantity);
            book.ChangeOwners(updateCommand.OwnerIds);
            book.ChangeAuthors(updateCommand.AuthorIds);

            var updatedBookId = await _booksRepository.Update(book);

            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(updatedBookId);
        }

        public async Task<Result<Guid>> DeleteBook(Guid id)
        {
            var bookEntity = await _booksRepository.GetById(id);
            if (bookEntity != null)
            {
                await _booksRepository.Delete(bookEntity);

                await _unitOfWork.CommitAsync();
            }

            return Result<Guid>.Success(id);
        }

        public async Task<Result<PagedResult<BookShortDTO>>> GetPagedBooks(BookQueryParameters parameters)
        {
            var pagedResult = await _booksReadRepository.GetPaged(parameters);

            return Result<PagedResult<BookShortDTO>>.Success(pagedResult);
        }

        //public async Task<Guid> ChangeBookOwners(Guid bookId, List<Guid> ownerIds)
        //{
        //    var entity = await _bookRepository.GetById(bookId);

        //    entity.ChangeOwners(ownerIds);

        //    return await _bookRepository.Update(entity);
        //}
    }
}
