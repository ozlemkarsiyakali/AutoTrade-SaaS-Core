using AutoTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTrade.Infrastructure.Configurations;

public class AppRoleClaimConfiguration : IEntityTypeConfiguration<AppRoleClaim>
{
    public void Configure(EntityTypeBuilder<AppRoleClaim> builder)
    {
        builder.ToTable("RoleClaims");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClaimType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ClaimValue).IsRequired().HasMaxLength(100);

        builder.HasIndex(x => x.RoleId);
    }
}