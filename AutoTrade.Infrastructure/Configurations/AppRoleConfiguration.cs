using AutoTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTrade.Infrastructure.Configurations;

public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(200);

        builder.HasMany(x => x.RoleClaims)
               .WithOne(x => x.Role)
               .HasForeignKey(x => x.RoleId);

        builder.HasMany(x => x.UserRoles)
               .WithOne(x => x.Role)
               .HasForeignKey(x => x.RoleId);

        builder.HasIndex(x => x.Name)
               .IsUnique()
               .HasFilter("\"IsDeleted\" = false");
    }
}