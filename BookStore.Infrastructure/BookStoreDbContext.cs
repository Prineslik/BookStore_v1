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
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<PermissionModel> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Применяем конфигурацию
            //modelBuilder.ApplyConfiguration(new BookConfiguration());

            // Если будет много конфигураций, лучше так:
             modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookStoreDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}