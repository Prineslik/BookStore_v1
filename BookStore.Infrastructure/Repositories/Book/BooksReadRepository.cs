using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Books.Queries;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Interfaces.Books;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BookStore.Infrastructure.Repositories.Book
{
    public class BooksReadRepository : IBooksReadRepository
    {
        private readonly BookStoreDbContext _context;

        public BooksReadRepository(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookShortDTO>> GetAll()
        {
            var bookDTO = await _context.Books
                .AsNoTracking()
                .Select(BookPersistenceMapping.ToListItemDTOExpr)
                .ToListAsync();

            return bookDTO;
        }
        
        public async Task<BookDetailsDTO?> GetById(Guid id)
        {
            var bookDTO = await _context.Books.AsNoTracking()
                //.Include(b => b.UserBookModels)
                //.Include(b => b.AuthorBookModels)
                .Where(b => b.Id == id)
                .Select(BookPersistenceMapping.ToDetailsDTOExpr)
                .FirstOrDefaultAsync();

            return bookDTO;
        }

        public async Task<List<BookShortDTO>> GetByTitle(string title)
        {
            var bookDTOs = _context.Books
                .AsNoTracking()
                .Where(b => b.Title.ToLower().Contains(title.ToLower()))
                .OrderBy(b => b.Title)
                .Select(BookPersistenceMapping.ToListItemDTOExpr)
                .ToList();

            return bookDTOs;
        }

        public async Task<BookDetailsDTO?> GetByTitleStrict(string title)
        {
            var bookDTO = await _context.Books
                .AsNoTracking()
                .Where(b => b.Title.ToLower().Equals(title.ToLower()))
                .Select(BookPersistenceMapping.ToDetailsDTOExpr)
                .FirstOrDefaultAsync();

            return bookDTO;
        }

        public async Task<bool> IsExistById(Guid id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }

        public async Task<bool> IsExistByTitle(string title)
        {
            return await _context.Books.AnyAsync(b => b.Title == title);
        }

        public async Task<PagedResult<BookShortDTO>> GetPaged(BookQueryParameters parameters)
        {
            var query = _context.Books
                .AsNoTracking()
                .AsQueryable();

            query = ApplyFilter(query, parameters);

            query = ApplySorting(query, parameters);

            var totalCount = await query.CountAsync(/*cancellationToken*/);

            var validPageSize = parameters.GetValidPageSize();
            var validPageNumber = parameters.PageNumber > 0 ? parameters.PageNumber : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(/*cancellationToken*/);

            return new PagedResult<BookShortDTO>
            {
                Items = items.AsQueryable()
                    .Select(BookPersistenceMapping.ToListItemDTOExpr)
                    .ToList(),
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
