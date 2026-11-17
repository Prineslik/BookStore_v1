using BookStore.Application.Contracts.Books.DTO;
using BookStore.Application.Contracts.Permissions.DTO;
using BookStore.Application.Contracts.Roles.DTO;
using BookStore.Core.Entities;
using BookStore.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BookStore.Infrastructure.Mappings
{
    public static class PermissionPersistanceMapping
    {
        public static PermissionModel ToModel(this PermissionEntity entity)
            => new PermissionModel 
            {
                Id = entity.Id,
                Code = entity.Code,
                Description = entity.Description,
                PermissionRoleModels = entity.RoleIds.Select(rId => new PermissionRoleModel 
                {
                    PermissionId = entity.Id,
                    RoleId = rId
                } ).ToList()
            };

        public static PermissionEntity ToDomain(this PermissionModel model)
            => PermissionEntity.Create(
                model.Code,
                model.Description);

        public static readonly Expression<Func<PermissionModel, PermissionDetailsDTO>> ToDetailsDTOExpr = model
           => new PermissionDetailsDTO(
               model.Id,
               model.Code,
               model.Description,
               model.PermissionRoleModels.Select(pr => new RoleShortDTO(
                   pr.RoleId,
                   pr.Role.Name))
               .ToList());

        public static readonly Expression<Func<PermissionModel, PermissionShortDTO>> ToListItemDTOExpr = model
            => new PermissionShortDTO(
                model.Id,
                model.Code,
                model.Description);
    }
}
