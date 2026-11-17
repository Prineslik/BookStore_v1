using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Permissions.DTO;
using BookStore.Application.Contracts.Roles.DTO;
using BookStore.Application.Contracts.Users.DTO;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BookStore.Infrastructure.Mappings
{
    public static class RolePersistanceMapping
    {
        public static RoleModel ToModel(this RoleEntity entity)
            => new RoleModel 
            { 
                Id = entity.Id,
                Name = entity.Name,
                PermissionRoleModels = entity.PermissionIds.Select(
                    permissionId => new PermissionRoleModel 
                    { 
                        PermissionId = permissionId,
                        RoleId = entity.Id
                    }).ToList(),
                UserRoleModels = entity.UserIds.Select(
                    userId => new UserRoleModel 
                    {
                        UserId = userId,
                        RoleId = entity.Id
                    }).ToList()
            };

        public static RoleEntity ToDomain(this RoleModel model)
            => RoleEntity.Reconstitute(
                model.Id,
                model.Name,
                model.PermissionRoleModels.Select(pr => pr.PermissionId).ToList(),
                model.UserRoleModels.Select(ur => ur.UserId).ToList());

        public static readonly Expression<Func<RoleModel, RoleDetailsDTO>> ToDetailsDTOExpr = model
           => new RoleDetailsDTO(
               model.Id,
               model.Name,
               model.PermissionRoleModels.Select(pr => new PermissionShortDTO(
                   pr.PermissionId,
                   pr.Permission.Code,
                   pr.Permission.Description))
               .ToList(),
               model.UserRoleModels.Select(pr => new UserShortDTO(
                   pr.UserId,
                   pr.User.UserName,
                   pr.User.Email,
                   pr.User.ProfilePhotoURL,
                   pr.User.IsAuthor))
               .ToList());

        public static readonly Expression<Func<RoleModel, RoleShortDTO>> ToListItemDTOExpr = model
            => new RoleShortDTO(
                model.Id,
                model.Name);
    }
}
