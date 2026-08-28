using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<PermissionModel>
    {
        public void Configure(EntityTypeBuilder<PermissionModel> builder)
        {
            builder.HasKey(p => p.Id);

            builder
                .Property(p => p.Code)
                .IsRequired();

            builder
                .Property(p => p.Description)
                .IsRequired();

            builder
               .HasMany(p => p.Roles)
               .WithMany(r => r.Permissions);
        }
    }
}
