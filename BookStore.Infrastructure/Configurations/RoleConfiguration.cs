using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<RoleModel>
    {
        public void Configure(EntityTypeBuilder<RoleModel> builder)
        {
            builder.HasKey(r => r.Id);

            builder
                .Property(r => r.Name)
                .IsRequired();

            builder
                .HasMany(r => r.Users)
                .WithMany(u => u.Roles);

            builder
               .HasMany(r => r.Permissions)
               .WithMany(p => p.Roles);
        }
    }
}
