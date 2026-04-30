using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Interfaces.Books;
using BookStore.Core.Entities;
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

        public async Task<List<BookEntity>> GetBooksByTitle(string title)
        { 
            return await _bookRepository.GetByTitle(title);
        }

        public async Task<Guid> CreateBook(BookEntity book)
        {
            return await _bookRepository.Create(book);
        }

        public async Task<Guid> UpdateBook(BookEntity book/*Guid id, string title, string description, decimal price*/)
        {
            //проверка на наличие в БД
            return await _bookRepository.Update(book);
        }

        public async Task<Guid> DeleteBook(Guid id)
        {
            return await _bookRepository.Delete(id);
        }

        public Task<PagedResult<BookEntity>> GetPagedBookAsync(BookQueryParameters parameters)
        {
            return _bookRepository.GetPagedAsync(parameters);
        }
    }
}
