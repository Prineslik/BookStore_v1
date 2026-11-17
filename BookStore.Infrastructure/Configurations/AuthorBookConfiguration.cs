using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Configurations
{
    public class AuthorBookConfiguration : IEntityTypeConfiguration<AuthorBookModel>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AuthorBookModel> builder)
        {
            builder.ToTable("AuthorBook");
            builder.HasKey(ab => new { ab.AuthorId, ab.BookId });

            builder.HasOne(ab => ab.Author)
                .WithMany(a => a.AuthorBookModels)
                .HasForeignKey(ab => ab.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ab => ab.Book)
                .WithMany(b => b.AuthorBookModels)
                .HasForeignKey(ab => ab.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
