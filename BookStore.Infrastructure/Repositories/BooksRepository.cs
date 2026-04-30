using AutoMapper;
using AutoMapper.Internal;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Interfaces.Books;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper _mapper;

        public BooksRepository(BookStoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BookEntity?>> GetAll()
        {
            var bookModels = await _context.Books.AsNoTracking().ToListAsync();

            //if (bookModels.Count == 0)
            //    return new List<BookEntity?>();

            var books = _mapper.Map<List<BookEntity>>(bookModels);

            return books;
        }

        public async Task<Guid> Create(BookEntity newBookEntity)
        {
            var newBookModel = _mapper.Map<BookModel>(newBookEntity);

            await _context.AddAsync(newBookModel);
            await _context.SaveChangesAsync();

            return newBookModel.Id;
        }

        public async Task<Guid> Update(BookEntity updatedBookEntity)
        {
            await _context.Books
                .Where(b => b.Id == updatedBookEntity.Id)
                .ExecuteUpdateAsync(u => u
                .SetProperty(f => f.Title, updatedBookEntity.Title)
                .SetProperty(f => f.Description, updatedBookEntity.Description)
                .SetProperty(f => f.Price, updatedBookEntity.Price));

            return updatedBookEntity.Id;
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
            var bookModel = await _context.Books
                .FindAsync(id);

            return _mapper.Map<BookEntity?>(bookModel);
        }

        public async Task<List<BookEntity?>> GetByTitle(string title)
        {
            var entityBook = _context.Books
                .Where(b => b.Title.ToLower().Contains(title.ToLower())).OrderBy(b => b.Title).ToList();


            var book = entityBook.Select(b => _mapper.Map<BookEntity>(b)).ToList();

            return book;
        }

        public async Task<PagedResult<BookEntity?>> GetPagedAsync(BookQueryParameters parameters/*, CancellationToken cancellationToken = default*/)
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
                Items = _mapper.Map<List<BookEntity>>(items),
                TotalCount = totalCount,
                PageNumber = validPageNumber,
                PageSize = validPageSize
            };
        }

        private IQueryable<BookModel> ApplyFilter(IQueryable<BookModel> query, BookQueryParameters parameters)
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

        private IQueryable<BookModel> ApplySorting(IQueryable<BookModel> query, BookQueryParameters parameters)
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
