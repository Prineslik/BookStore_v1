using AutoMapper;
using AutoMapper.Internal;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Books;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksRepository> _logger;

        public BooksRepository(BookStoreDbContext context, IMapper mapper, ILogger<BooksRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<BookEntity>> GetAll()
        {
            _logger.LogDebug($"Получение всех книг");

            var bookModels = await _context.Books.AsNoTracking().ToListAsync();

            var bookEntities = _mapper.Map<List<BookEntity>>(bookModels);

            _logger.LogDebug($"Все книги получены");

            return bookEntities;
            //return Result<List<BookEntity?>>.Success(bookEntities);
        }

        public async Task<Guid> Create(BookEntity newBookEntity)
        {
            try
            {
                _logger.LogDebug("Создание книги с GUID = {@BookId}", newBookEntity.Id);

                var newBookModel = _mapper.Map<BookModel>(newBookEntity);

                await _context.AddAsync(newBookModel);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Книга с GUID = {newBookEntity.Id} создана");

                return newBookModel.Id;//return Result<Guid>.Success(newBookModel.Id);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogWarning($"Книга с GUID = {newBookEntity.Id} не создана");

                throw new DuplicateException($"Book with GUID {newBookEntity.Id} already exists");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning($"DB exception");

                throw new InfrastructureException("Failed to save book. " + ex.Message);
            }
        }

        public async Task<Guid> Update(BookEntity bookEntity)
        {
            try
            {
                _logger.LogDebug($"Изменение книги с GUID = {bookEntity.Id}");

                var updatedBook = await _context.Books
                    .Where(b => b.Id == bookEntity.Id)
                    .ExecuteUpdateAsync(u => u
                    .SetProperty(f => f.Title, bookEntity.Title)
                    .SetProperty(f => f.Description, bookEntity.Description)
                    .SetProperty(f => f.Price, bookEntity.Price));

                _logger.LogDebug($"Книга с GUID = {bookEntity.Id} изменена");

                //return updatedBook > 0 ? Result<Guid>.Success(bookEntity.Id) : Result<Guid>.Failure(Error.Unexpected($"Ошибка при внесении изменеий в книгу {bookEntity.Title}"));
                return bookEntity.Id;
            }
            catch (DbUpdateException ex)
            {
                throw new InfrastructureException($"Failed to update book with GUID = {bookEntity.Id}\n{ex.Message}");
            }
        }

        public async Task<Guid> Delete(Guid id)
        {
            try
            {
                _logger.LogDebug($"Удаление книги с GUID = {id}");

                /*var resultDelete =*/
                await _context.Books
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();

                _logger.LogDebug($"Книга с GUID = {id} удалена");

                return id;
            }
            catch (DbUpdateException ex)
            {
                throw new InfrastructureException($"Failed to delete book with GUID = {id}\n{ex.Message}");
            }
            /*return resultDelete > 0 
                ? Result<Guid>.Success(id) 
                : Result<Guid>.Failure(Error.NotFound("Book", "Id", ""));*/
        }

        public async Task<BookEntity?> GetById(Guid id)
        {
            _logger.LogDebug("Полученеи книги с GUID = {@BookId}", id);

            var bookModel = await _context.Books
                .FindAsync(id);

            if(bookModel == null)
                throw new NotFoundException("Book", id);

            _logger.LogDebug($"Книга с GUID = {id} получена");

            return _mapper.Map<BookEntity?>(bookModel);
            //Result<BookEntity?>.Failure(Error.NotFound("Book", "Id", id));
        }

        public async Task<List<BookEntity>> GetByTitle(string title)
        {
            _logger.LogDebug("Полученеи всех книги с названием = {@Title}", title);

            var bookModels = _context.Books
                .AsNoTracking()
                .Where(b => b.Title.ToLower().Contains(title.ToLower()))
                .OrderBy(b => b.Title)
                .ToList();

            _logger.LogDebug("Получены все книги с названием = {@Title}", title);

            /*if (bookModels.Count == 0)
                return Result<List<BookEntity?>>.Failure(Error.NotFound("Books", "Title", title));*/

            var bookEntities = _mapper.Map<List<BookEntity>>(bookModels);

            return bookEntities;
            //return Result<List<BookEntity?>>.Success(bookEntities);
        }

        public async Task<bool> IsExist(Guid id)
        {
            return await _context.Books.FindAsync(id) != null ? true : false;
        }

        public async Task<PagedResult<BooksResponse>> GetPaged(BookQueryParameters parameters/*, CancellationToken cancellationToken = default*/)
        {
            _logger.LogDebug("Полученеи книг с параметрами = {@Params}", parameters);

            var query = _context.Books.AsQueryable();

            query = ApplyFilter(query, parameters);

            query = ApplySorting(query, parameters);

            var totalCount = await query.CountAsync(/*cancellationToken*/);

            //if (totalCount == 0)
            //    return Result<PagedResult<BooksResponse?>>.Failure(Error.NotFound("Book", "SearchTerm", parameters.SearchTerm));

            var validPageSize = parameters.GetValidPageSize();
            var validPageNumber = parameters.PageNumber > 0 ? parameters.PageNumber : 1;

            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync(/*cancellationToken*/);

            _logger.LogDebug("Книги с параметрами получены");

            return new PagedResult<BooksResponse>
            {
                Items = _mapper.Map<List<BooksResponse>>(items),
                TotalCount = totalCount,
                PageNumber = validPageNumber,
                PageSize = validPageSize
            };

            /*return Result<PagedResult<BooksResponse?>>.Success(
                new PagedResult<BooksResponse>
            {
                Items = _mapper.Map<List<BooksResponse>>(items),
                TotalCount = totalCount,
                PageNumber = validPageNumber,
                PageSize = validPageSize
            });*/
        }

        private IQueryable<BookModel> ApplyFilter(IQueryable<BookModel> query, BookQueryParameters parameters)
        {
            _logger.LogDebug("Применение параметров фильтрации поиска Books");

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

            _logger.LogDebug("Параметры фильтрации поиска Books успешно применены");

            return query;
        }

        private IQueryable<BookModel> ApplySorting(IQueryable<BookModel> query, BookQueryParameters parameters)
        {
            _logger.LogDebug("Применение параметров сортировки поиска Books");

            if (string.IsNullOrWhiteSpace(parameters.SortBy))
            {
                return query.OrderBy(p => p.Id);
            }

            _logger.LogDebug("Параметры сортировки поиска Books успешно применены");

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
