using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Contracts.Users;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Application.Interfaces.Books;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Services
{
    public class BooksService : IBooksService
    {
        private readonly IBooksRepository _bookRepository;

        public BooksService(IBooksRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<BookEntity>> GetAllBooks()
        {
            return await _bookRepository.GetAll();
        }
        public async Task<BookEntity?> GetBookById(Guid id)
        {
            return await _bookRepository.GetById(id);
        }

        public async Task<List<BookEntity?>> GetBooksByTitle(string title)
        { 
            return await _bookRepository.GetByTitle(title);
        }

        public async Task<Guid> CreateBook(BookEntity bookEntity)
        {
            var existingBook = await _bookRepository.GetByTitle(bookEntity.Title);

            if (existingBook != null)
                throw new DuplicateException($"Book с Title = {bookEntity.Title} уже существует");

            var createdBookId = await _bookRepository.Create(bookEntity);
            return createdBookId;
        }

        public async Task<Guid> UpdateBook(BookEntity bookEntity)
        {
            //проверка на наличие в БД
            var bookByIdResult = await _bookRepository.GetById(bookEntity.Id);

            if (bookByIdResult == null)
                throw new NotFoundException("Book", bookEntity.Id); ;

            return await _bookRepository.Update(bookEntity);
            //return string.IsNullOrEmpty(error) 
            //    ? await _bookRepository.Update(bookEntity)
            //    : Result<Guid>.Failure(Error.Validation(error));
        }

        public async Task<Guid> DeleteBook(Guid id)
        {

            if (!await _bookRepository.IsExist(id))
                throw new NotFoundException("Book", id);

            return await _bookRepository.Delete(id);
        }

        public Task<PagedResult<BooksResponse>> GetPagedBooks(BookQueryParameters parameters)
        {
            return _bookRepository.GetPaged(parameters);
        }
    }
}
