using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<AuthorModel>
    {
        public void Configure(EntityTypeBuilder<AuthorModel> builder)
        {
            builder.ToTable("Authors");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired();

            builder.HasOne(a => a.User)
                .WithOne(u => u.Author)
                .HasForeignKey<AuthorModel>(a => a.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
