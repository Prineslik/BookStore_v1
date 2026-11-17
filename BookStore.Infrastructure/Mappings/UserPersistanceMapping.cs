using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Permissions.DTO;
using BookStore.Application.Contracts.Roles.DTO;
using BookStore.Application.Contracts.Users.DTO;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace BookStore.Infrastructure.Mappings
{
    public static class UserPersistanceMapping
    {
        public static UserModel ToModel(this UserEntity entity)
            => new UserModel
            { 
                Id = entity.Id,
                UserName = entity.UserName,
                Email = entity.Email,
                PasswordHash = entity.PasswordHash,
                ProfilePhotoURL = entity.ProfilePhotoURL,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                IsAuthor = entity.IsAuthor,
                AuthorId = entity.AuthorId,
                UserRoleModels = entity.RoleIds.Select(rId => new UserRoleModel 
                {
                    UserId = entity.Id,
                    RoleId = rId
                }).ToList(),
                UserBookModels = entity.OwnedBookIds.Select(bId => new UserBookModel 
                {
                    UserId = entity.Id,
                    BookId = bId
                }).ToList()
            };

        //public static UserEntity ToDomain(this UserModel model)
        //    => UserEntity.Create(//Reconstitute
        //       model.UserName,
        //       model.Email,
        //       model.PasswordHash,
        //       model.ProfilePhotoURL,
        //       model.AuthorId,
        //       model.UserRoleModels.Select(ur => ur.RoleId).ToList(),
        //       model.UserBookModels.Select(ub => ub.BookId).ToList());

        //public static UserDetailsDTO ToDetailsDTO(this UserModel model)
        //    => new UserDetailsDTO(
        //        model.Id,
        //        model.UserName,
        //        model.Email,
        //        model.ProfilePhotoURL,
        //        model.IsAuthor,
        //        model.AuthorId,
        //        model.UserRoleModels.Select(ur => ur.RoleId).ToList(),
        //        model.UserBookModels.Select(ub => ub.BookId).ToList());

        public static readonly Expression<Func<UserModel, UserDetailsDTO>> ToDetailsDTOExpr = model
           => new UserDetailsDTO(
                model.Id,
                model.UserName,
                model.Email,
                model.ProfilePhotoURL,
                model.IsAuthor,
                model.AuthorId,
                model.UserRoleModels.Select(ur => new RoleShortDTO(
                    ur.RoleId,
                    ur.Role.Name))
               .ToList(),
                model.UserBookModels.Select(ub => new BookShortDTO(
                    ub.BookId,
                    ub.Book.Title,
                    ub.Book.Description,
                    ub.Book.Price,
                    ub.Book.StockQuantity,
                    ub.Book.CreateDate,
                    ub.Book.UpdateDate))
               .ToList());

        //public static readonly Expression<Func<UserModel, UserShortDTO>> ToShortDTOExpr = model
        //   => new UserShortDTO(
        //        model.Id,
        //        model.UserName,
        //        model.Email,
        //        model.ProfilePhotoURL,
        //        model.IsAuthor);

        public static readonly Expression<Func<UserModel, UserShortDTO>> ToListItemDTOExpr = model
            => new UserShortDTO(
                model.Id,
                model.UserName,
                model.Email,
                model.ProfilePhotoURL,
                model.IsAuthor);

        public static readonly Expression<Func<UserModel, UserAuthDTO>> ToAuthDTOExpr = model
            => new UserAuthDTO(
                model.Id,
                model.Email,
                model.PasswordHash,
                model.UserRoleModels.Select(ur => new RoleShortDTO(
                    ur.RoleId,
                    ur.Role.Name))
                .ToList());
    }
}
