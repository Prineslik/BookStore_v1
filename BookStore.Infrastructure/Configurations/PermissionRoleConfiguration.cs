using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Infrastructure.Configurations
{
    public class PermissionRoleConfiguration : IEntityTypeConfiguration<PermissionRoleModel>
    {
        public void Configure(EntityTypeBuilder<PermissionRoleModel> builder)
        {
            builder.ToTable("PermissionModelRoleModel");
            builder.HasKey(pr => new { pr.PermissionId, pr.RoleId });

            builder.HasOne(pr => pr.Permission)
                .WithMany(p => p.PermissionRoleModels)
                .HasForeignKey(pr => pr.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pr => pr.Role)
                .WithMany(r => r.PermissionRoleModels)
                .HasForeignKey(pr => pr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
