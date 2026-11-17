using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Configurations
{
    public class UserBookConfiguration : IEntityTypeConfiguration<UserBookModel>
    {
        public void Configure(EntityTypeBuilder<UserBookModel> builder)
        {
            builder.ToTable("UserBook");
            builder.HasKey(ub => new { ub.UserId, ub.BookId });

            builder.HasOne(ab => ab.User)
                .WithMany(a => a.UserBookModels)
                .HasForeignKey(ab => ab.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ab => ab.Book)
                .WithMany(b => b.UserBookModels)
                .HasForeignKey(ab => ab.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
