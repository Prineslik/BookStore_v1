using BookStore.Application.Contracts.Authors.DTO;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Users.DTO;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BookStore.Infrastructure.Mappings
{
    public static class BookPersistenceMapping
    {
        public static BookModel ToModel(this BookEntity entity)
            => new BookModel 
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Price = entity.Price,
                StockQuantity = entity.StockQuantity,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                UserBookModels = entity.OwnerIds.Select(ownerId => new UserBookModel 
                {
                    UserId = ownerId,
                    BookId = entity.Id
                }).ToList(),
                AuthorBookModels = entity.AuthorIds.Select(authorId => new AuthorBookModel
                {
                    AuthorId = authorId,
                    BookId = entity.Id
                }).ToList()
            };

        public static BookEntity ToDomain(this BookModel model)
            => BookEntity.Reconstitute(
                model.Id,
                model.Title,
                model.Description,
                model.Price,
                model.StockQuantity,
                model.CreateDate,
                model.UpdateDate,
                model.UserBookModels.Select(ub => ub.UserId).ToList(),
                model.AuthorBookModels.Select(ub => ub.AuthorId).ToList());

        public static readonly Expression<Func<BookModel, BookDetailsDTO>> ToDetailsDTOExpr = model
            => new BookDetailsDTO(
                model.Id,
                model.Title,
                model.Description,
                model.Price,
                model.StockQuantity,
                model.CreateDate,
                model.UpdateDate,
                model.UserBookModels.Select(ub => new OwnerShortDTO(
                    ub.UserId,
                    ub.User.UserName,
                    ub.User.Email,
                    ub.User.ProfilePhotoURL,
                    ub.User.IsAuthor))
                .ToList(),
                model.AuthorBookModels.Select(ab => new AuthorShortDTO(
                    ab.AuthorId,
                    ab.Author.Name,
                    ab.Author.PhotoUrl))
                .ToList()
            );

        public static readonly Expression<Func<BookModel, BookShortDTO>> ToListItemDTOExpr = model
            => new BookShortDTO(
                model.Id,
                model.Title,
                model.Description,
                model.Price,
                model.StockQuantity,
                model.CreateDate,
                model.UpdateDate);
    }
}
