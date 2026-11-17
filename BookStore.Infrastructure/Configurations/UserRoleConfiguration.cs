using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRoleModel>
    {
        public void Configure(EntityTypeBuilder<UserRoleModel> builder)
        {
            builder.ToTable("RoleModelUserModel");
            builder.HasKey(ru => new { ru.RoleId, ru.UserId });

            builder.HasOne(ru => ru.Role)
                .WithMany(r => r.UserRoleModels)
                .HasForeignKey(pr => pr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ru => ru.User)
                .WithMany(u => u.UserRoleModels)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
