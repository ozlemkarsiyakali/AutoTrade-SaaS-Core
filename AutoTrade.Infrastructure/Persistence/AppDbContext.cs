using System.Reflection;
using AutoTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoTrade.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<VehicleBrand> VehicleBrands => Set<VehicleBrand>();
    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
    public DbSet<VehicleModelNode> VehicleModelNodes => Set<VehicleModelNode>();
    public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
    public DbSet<AppRole> Roles => Set<AppRole>();
    public DbSet<AppRoleClaim> RoleClaims => Set<AppRoleClaim>();
    public DbSet<AppUserRole> UserRoles => Set<AppUserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurations klasöründeki Fluent API ayarlarını otomatik yükler
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 1. AppRole haritalamasını garantiye al (Fazla 'Role' tablosu oluşmasını engeller)
        modelBuilder.Entity<AppRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.Id);
        });

        // 2. User ve AppRole arasındaki Foreign Key ilişkisini 'Roles' tablosuna bağla
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasOne<AppRole>()
                  .WithMany()
                  .HasForeignKey(u => u.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 3. Global Soft-Delete Query Filter (IsDeleted = false olanları otomatik filtreler)
        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<AppRole>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<VehicleBrand>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<VehicleModel>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<VehicleModelNode>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<VehicleType>().HasQueryFilter(x => !x.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}