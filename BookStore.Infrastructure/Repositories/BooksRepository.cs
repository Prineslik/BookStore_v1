using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly BookStoreDbContext _context;

        public BooksRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookEntity>> GetAll()
        {
            var bookEntities = await _context.Books.AsNoTracking().ToListAsync();

            var books = bookEntities
                .Select(b => BookEntity.Create(b.Id, b.Title, b.Description, b.Price).Book)
                .ToList();

            return books;
        }

        public async Task<Guid> Create(Guid id, string title, string description, decimal price)
        {
            var book = BookEntity.Create(id, title, description, price).Book;

            await _context.AddAsync(book);
            await _context.SaveChangesAsync();

            return book.Id;
        }

        public async Task<Guid> Update(Guid id, string title, string description, decimal price)
        {
            await _context.Books
                .Where(b => b.Id == id)
                .ExecuteUpdateAsync(u => u
                .SetProperty(f => f.Title, title)
                .SetProperty(f => f.Description, description)
                .SetProperty(f => f.Price, price));

            return id;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _context.Books
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }

        public async Task<BookEntity?> GetById(Guid id)
        {
            var book = await _context.Books
                .FindAsync(id);

            //var book = Book.Create(entityBook.Id, entityBook.Title, entityBook.Description, entityBook.Price).Book;

            return book;
        }

        public async Task<List<BookEntity>> GetByTitle(string title)
        {
            var entityBook = _context.Books
                .Where(b => b.Title.ToLower().Contains(title.ToLower())).OrderBy(b => b.Title).ToList();


            var book = entityBook.Select(b => BookEntity.Create(b.Id, b.Title, b.Description, b.Price).Book).ToList();//Book.Create(entityBook.Id, entityBook.Title, entityBook.Description, entityBook.Price).Book;

            return book;
        }

        public async Task<PagedResult<BookEntity>> GetPagedAsync(ProductQueryParameters parameters/*, CancellationToken cancellationToken = default*/)
        {
            var query = _context.Books.AsQueryable();

            query = ApplyFilter(query, parameters);

            query = ApplySorting(query, parameters);

            var totalCount = await query.CountAsync(/*cancellationToken*/);

            var validPageSize = parameters.GetValidPageSize();
            var validPageNumber = parameters.PageNumber > 0 ? parameters.PageNumber : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(/*cancellationToken*/);

            return new PagedResult<BookEntity>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = validPageNumber,
                PageSize = validPageSize
            };
        }

        private IQueryable<BookEntity>ApplyFilter(IQueryable<BookEntity> query, ProductQueryParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm));
            }

            if (parameters.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= parameters.MaxPrice.Value);
            }

            //if (parameters.CategoryId.HasValue)
            //{
            //    query = query.Where(p => p.CategoryId == parameters.CategoryId.Value);
            //}

            if (parameters.IsInStock.HasValue)
            {
                query = parameters.IsInStock.Value
                    ? query.Where(p => p.StockQuantity > 0)
                    : query.Where(p => p.StockQuantity == 0);
            }

            return query;
        }

        private IQueryable<BookEntity> ApplySorting(IQueryable<BookEntity> query, ProductQueryParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                return query.OrderBy(p => p.Id);
            }

            return parameters.SortBy.ToLower() switch
            {
                "title" => parameters.SortDescending
                ? query.OrderByDescending(p => p.Title)
                : query.OrderBy(p => p.Title),

                "price" => parameters.SortDescending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),

                _ => parameters.SortDescending
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id)
            };
         }
    }
}
