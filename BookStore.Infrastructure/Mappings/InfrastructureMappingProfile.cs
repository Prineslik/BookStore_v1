using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Permissions;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using BookStore.Infrastructure.Models;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Mappings
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            CreateMap<BookEntity, BookModel>().ReverseMap();

            CreateMap<BookEntity, BooksResponse>()
                .ReverseMap();

            CreateMap<BookModel, BooksResponse>()
                .ReverseMap();
            //CreateMap<BooksRequest, BookEntity>().ReverseMap();

            CreateMap<UserEntity, UserModel>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
                .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore());

            CreateMap<UserModel, UserEntity>(MemberList.None)
                .ConstructUsing(src => 
                    UserEntity.Create(
                        src.Id, 
                        src.UserName ?? "<blank_name>", 
                        src.Email ?? "<blank_email>", 
                        src.PasswordHash ?? "<blank_passwordhash>",
                        src.ProfilePhotoURL,
                        (src.Roles ?? Enumerable.Empty<RoleModel>())
                            .Where(r => r != null)
                            .Select(r => (Guid?)r.Id)
                        ).User);

            //.ForMember(dest => dest.Id,
            //    opt => opt.MapFrom(src => src.Id))
            //.ForMember(dest => dest.Name,
            //    opt => opt.MapFrom(src => src.UserName))
            //.ForMember(dest => dest.Email,
            //    opt => opt.MapFrom(src => src.Email))
            //.ForMember(dest => dest.PasswordHash,
            //    opt => opt.MapFrom(src => src.PasswordHash))
            //.ForMember(dest => dest.ProfilePhotoURL,
            //    opt => opt.MapFrom(src => src.ProfilePhotoURL));

            //CreateMap<UserEntity, UsersResponse>()
            //    .ForMember(dest => dest.Id,
            //        opt => opt.MapFrom(src => src.Id))
            //    .ForMember(dest => dest.Name,
            //        opt => opt.MapFrom(src => src.Name))
            //    .ForMember(dest => dest.Email,
            //        opt => opt.MapFrom(src => src.Email))
            //    .ForMember(dest => dest.PasswordHash,
            //        opt => opt.MapFrom(src => src.PasswordHash))
            //    .ForMember(dest => dest.ProfilePhotoURL,
            //        opt => opt.MapFrom(src => src.ProfilePhotoURL))
            //    .ReverseMap();

            CreateMap<UserEntity, UsersResponse>().ReverseMap();

            //CreateMap<UsersRequest, UserEntity>(MemberList.None)
            //    .ConstructUsing(src =>
            //        UserEntity.Create(
            //            Guid.NewGuid,
            //            src.Name ?? "<blank_name>",
            //            src.Email ?? "<blank_email>",
            //            src.PasswordHash ?? "<blank_passwordhash>",
            //            src.ProfilePhotoURL,
            //            (src.Roles ?? Enumerable.Empty<RoleModel>())
            //                .Where(r => r != null)
            //                .Select(r => (Guid?)r.Id)
            //            ).User);

                //.ForMember(dest => dest.Id, 
                //    opt => opt.MapFrom(src => src.Id))
                //.ForMember(dest => dest.Name, 
                //    opt => opt.MapFrom(src => src.Name))
                //.ForMember(dest => dest.Email, 
                //    opt => opt.MapFrom(src => src.Email))
                //.ForMember(dest => dest.PasswordHash, 
                //    opt => opt.MapFrom(src => src.PasswordHash))
                //.ForMember(dest => dest.ProfilePhotoURL, 
                //    opt => opt.MapFrom(src => src.ProfilePhotoURL));

            CreateMap<RoleModel, RolesResponse>(MemberList.None)
                .ConstructUsing(src => new RolesResponse(
                    src.Id,
                    src.Name,
                    (src.Permissions ?? Enumerable.Empty<PermissionModel?>()).Select(p => (Guid?)p.Id).ToList()
                ));

            CreateMap<RolesRequest, RoleEntity>();

            CreateMap<RoleEntity, RolesResponse>();
                //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                //.ForMember(dest => dest.PermissionIds, opt => opt.MapFrom(src => src.PermissionIds));

            CreateMap<RoleEntity, RoleModel>()
                .ForMember(dest => dest.Id, 
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, 
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Permissions, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore());
            ;
            //Permissions нужно делать вручную после маппинга

            CreateMap<RoleModel, RoleEntity>(MemberList.None)
                .ConstructUsing(src =>
                RoleEntity.Create(
                    src.Id,
                    src.Name ?? "<blank_name>",
                    (src.Permissions ?? Enumerable.Empty<PermissionModel>())
                        .Where(p => p != null)
                        .Select(p => (Guid?)p.Id)
                        .ToList()
                ).RoleEntity);
                //.ForMember(dest => dest.Id, 
                //    opt => opt.MapFrom(src => src.Id))
                //.ForMember(dest => dest.Name, 
                //    opt => opt.MapFrom(src => src.Name))
                //.ForMember(dest => dest.PermissionIds, 
                //    opt => opt.MapFrom(src => src.Permissions.Select(p => p.Id).ToList()));

            CreateMap<PermissionModel, PermissionsResponse>();

            CreateMap<PermissionsRequest, PermissionEntity>();

            CreateMap<PermissionEntity, PermissionsResponse>();

            CreateMap<PermissionEntity, PermissionModel>()
                .ForMember(dest => dest.Roles,
                    opt => opt.Ignore());

            //Roles нужно делать вручную после маппинга

        }
    }
}
