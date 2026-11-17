using AutoMapper;
using AutoMapper.Internal;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Common;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces.Books;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Mappings;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Repositories.Book
{
    public class BooksRepository : IBooksRepository
    {
        private readonly BookStoreDbContext _context;
        private readonly ILogger<BooksRepository> _logger;

        public BooksRepository(BookStoreDbContext context, ILogger<BooksRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Create(BookEntity newBookEntity)
        {
            //try
            //{
            _logger.LogDebug("Создание книги с GUID = {@BookId}", newBookEntity.Id);

            var newBookModel = newBookEntity.ToModel();

            await _context.AddAsync(newBookModel);
            //await _context.SaveChangesAsync();

            _logger.LogInformation($"Книга с GUID = {newBookEntity.Id} создана");

            return newBookModel.Id;
            //}
            //catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            //{
            //    _logger.LogWarning($"Книга с GUID = {newBookEntity.Id} не создана");

            //    throw new DuplicateException($"Book with GUID {newBookEntity.Id} already exists");
            //}
            //catch (DbUpdateException ex)
            //{
            //    _logger.LogWarning($"DB exception");

            //    throw new InfrastructureException("Failed to save book. " + ex.Message);
            //}
        }

        public async Task<Guid> Update(BookEntity bookEntity)
        {
            //_logger.LogDebug($"Изменение книги с GUID = {entity.Id}");

            var bookModel = await _context.Books
                .Where(b => b.Id == bookEntity.Id)
                .Include(b => b.AuthorBookModels)
                .Include(b => b.UserBookModels)
                .FirstOrDefaultAsync();

            if (bookModel is null)
                throw new InvalidOperationException(
                $"Cannot update Book {bookEntity.Id}: not found in database");

            bookModel.Title = bookEntity.Title;
            bookModel.Description = bookEntity.Description;
            bookModel.Price = bookEntity.Price;
            bookModel.StockQuantity = bookEntity.StockQuantity;
            bookModel.UpdateDate = DateTime.UtcNow;

            SyncAuthors(bookModel, bookEntity.AuthorIds);
            SyncOwners(bookModel, bookEntity.OwnerIds);

            //await _context.SaveChangesAsync();
                
            //_logger.LogDebug($"Книга с GUID = {entity.Id} изменена");

            return bookEntity.Id;
            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    _logger.LogWarning(ex, "Конфликт обновления для Book с ID = {BookId}", entity.Id);

            //    var entry = ex.Entries.Single();
            //    var databaseValues = await entry.GetDatabaseValuesAsync();

            //    if (databaseValues == null)
            //    {
            //        throw new NotFoundException("Book", entity.Id);
            //    }

            //    throw new DbConcurrencyException($"Book с ID = {entity.Id} была изменена другим пользователем. Обновите данные.");
            //}
            //catch (DbUpdateException ex)
            //{
            //    throw new InfrastructureException($"Failed to update book with GUID = {entity.Id}\n{ex.Message}");
            //}
        }

        public async Task<Guid> Delete(BookEntity bookEntity)
        {
            //var book = await _context.Books
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(b => b.Id == id);

            //if (book != null)
            _context.Books.Remove(bookEntity.ToModel());

            return bookEntity.Id;
        }

        private void SyncOwners(BookModel model, IReadOnlyCollection<Guid> ownerIds)
        {
            var target = ownerIds.ToHashSet();

            model.UserBookModels.RemoveAll(ub => !target.Contains(ub.UserId));

            var current = model.UserBookModels.Select(ub => ub.UserId).ToHashSet();
            foreach (var ownerId in target.Except(current))
            {
                model.UserBookModels.Add(new UserBookModel
                {
                    UserId = ownerId
                });
            }
        }

        private void SyncAuthors(BookModel model, IReadOnlyCollection<Guid> authorIds)
        {
            var target = authorIds.ToHashSet();

            model.AuthorBookModels.RemoveAll(ab => !target.Contains(ab.AuthorId));

            var current = model.AuthorBookModels.Select(ab => ab.AuthorId).ToHashSet();
            foreach (var authorId in target.Except(current))
            {
                model.AuthorBookModels.Add(new AuthorBookModel
                {
                    AuthorId = authorId
                });
            }
        }

        public async Task<BookEntity?> GetById(Guid id)
        {
            var bookModel = await _context.Books
                .AsNoTracking()
                .Include(b => b.UserBookModels)
                .Include(b => b.AuthorBookModels)
                .FirstOrDefaultAsync(b => b.Id == id);

            var bookEntity = bookModel.ToDomain() ?? null;

            return bookEntity;
        }
    }
}
