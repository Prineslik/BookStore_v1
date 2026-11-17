using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Permissions;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Mappings
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {

            //CreateMap<AuthorEntity, UsersResponse>();

            //CreateMap<BookEntity, BooksResponse>();

            //CreateMap<UserEntity, UsersResponse>();

            //CreateMap<RoleEntity, RoleDeatailsDTO>();

            //CreateMap<PermissionEntity, PermissionsResponse>();
        }
    }
}