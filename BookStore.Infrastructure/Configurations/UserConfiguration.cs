using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.HasKey(u => u.Id);

            builder
                .Property(u => u.UserName)
                .HasMaxLength(250)
                .IsRequired();

            builder
                .Property(u => u.Email)
                .HasMaxLength(250)
                .IsRequired();

            builder
                .Property(u => u.PasswordHash)
                .IsRequired();

            builder
                .Property(u => u.ProfilePhotoURL)
                .IsRequired();

            builder
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users);
                
        }
    }
}
