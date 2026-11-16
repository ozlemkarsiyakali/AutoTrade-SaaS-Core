using AutoTrade.Core.Constants;
using AutoTrade.Domain.Entities;
using AutoTrade.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoTrade.Infrastructure.Seeds;

public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. SuperAdmin Rolü
        var superAdminRole = await context.Set<AppRole>().FirstOrDefaultAsync(r => r.Name == "SuperAdmin" && !r.IsDeleted);
        if (superAdminRole == null)
        {
            superAdminRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "SuperAdmin",
                Description = "Tam yetkili sistem yöneticisi Appden gelen"
            };
            await context.Set<AppRole>().AddAsync(superAdminRole);
        }

        // 2. Admin Rolü
        var adminRole = await context.Set<AppRole>().FirstOrDefaultAsync(r => r.Name == "Admin" && !r.IsDeleted);
        if (adminRole == null)
        {
            adminRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Description = "Yönetici rolü"
            };
            await context.Set<AppRole>().AddAsync(adminRole);
        }

        // 3. User Rolü (Kayıt olan kullanıcılara varsayılan atanan rol)
        var userRole = await context.Set<AppRole>().FirstOrDefaultAsync(r => r.Name == "User" && !r.IsDeleted);
        if (userRole == null)
        {
            userRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "User",
                Description = "Standart kullanıcı rolü"
            };
            await context.Set<AppRole>().AddAsync(userRole);
        }

        // Rollerin veritabanına yansımasını sağlayalım
        await context.SaveChangesAsync();

        // 4. SuperAdmin İçin Tüm İzinleri (Permissions) Ekle
        var existingClaims = await context.Set<AppRoleClaim>()
            .Where(c => c.RoleId == superAdminRole.Id && !c.IsDeleted)
            .Select(c => c.ClaimValue)
            .ToListAsync();

        var allPermissions = Permissions.GetAllPermissions();

        foreach (var permission in allPermissions)
        {
            if (!existingClaims.Contains(permission))
            {
                await context.Set<AppRoleClaim>().AddAsync(new AppRoleClaim
                {
                    RoleId = superAdminRole.Id,
                    ClaimType = "Permission",
                    ClaimValue = permission
                });
            }
        }

        await context.SaveChangesAsync();
    }
}