using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
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
            return await _bookRepository.Create(book.Id, book.Title, book.Description, book.Price);
        }

        public async Task<Guid> UpdateBook(Guid id, string title, string description, decimal price)
        {
            return await _bookRepository.Update(id, title, description, price);
        }

        public async Task<Guid> DeleteBook(Guid id)
        {
            return await _bookRepository.Delete(id);
        }

        public Task<PagedResult<BookEntity>> GetPagedBookAsync(ProductQueryParameters parameters)
        {
            return _bookRepository.GetPagedAsync(parameters);
        }
    }
}
