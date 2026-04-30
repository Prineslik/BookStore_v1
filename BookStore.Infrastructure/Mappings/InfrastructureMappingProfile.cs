using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
using BookStore.Core.Models;
using BookStore.Infrastructure.Models;
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
            //CreateMap<BooksResponse, BookEntity>().ReverseMap();
            //CreateMap<BooksRequest, BookEntity>().ReverseMap();

            CreateMap<UserEntity, UserModel>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.Name))
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

            CreateMap<UserModel, UserEntity>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.ProfilePhotoURL,
                    opt => opt.MapFrom(src => src.ProfilePhotoURL));

            CreateMap<UserEntity, UsersResponse>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.ProfilePhotoURL,
                    opt => opt.MapFrom(src => src.ProfilePhotoURL))
                .ReverseMap();

            //CreateMap<UsersRequest, UserEntity>()
            //    .ConstructUsing( src => UserEntity.Create(
            //        src.Name,
            //        src.));
        }
    }
}
