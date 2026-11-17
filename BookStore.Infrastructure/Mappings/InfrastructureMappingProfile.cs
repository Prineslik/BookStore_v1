using AutoMapper;
using BookStore.Application.Contracts.Books;
using BookStore.Application.Contracts.Permissions;
using BookStore.Application.Contracts.Roles;
using BookStore.Application.Contracts.Users;
using BookStore.Core.Entities;
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
            //пока ничего, Entity <-> Response/Request в Applicaion
        }
    }
}
