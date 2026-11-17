//using BookStore.Core.Entities;
using BookStore.Infrastructure.Configurations;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure
{
    public class BookStoreDbContext : DbContext
    {
        public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
            : base(options)
        {}

        public DbSet<BookModel> Books { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<AuthorModel> Authors { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<PermissionModel> Permissions { get; set; }

        public DbSet<AuthorBookModel> AuthorBook { get; set; }
        public DbSet<PermissionRoleModel> PermissionRole { get; set; }
        public DbSet<UserBookModel> UserBook { get; set; }
        public DbSet<UserRoleModel> UserRole { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new BookConfiguration());

             modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookStoreDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}