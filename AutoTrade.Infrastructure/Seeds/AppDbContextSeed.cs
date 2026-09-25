using AutoTrade.Core.Constants;
using AutoTrade.Core.Entities; 
using AutoTrade.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoTrade.Infrastructure.Seeds;

public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // 1. SuperAdmin Rolü Yoksa Ekle
        var adminRole = await context.Set<AppRole>().FirstOrDefaultAsync(r => r.Name == "SuperAdmin" && !r.IsDeleted);

        if (adminRole == null)
        {
            adminRole = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = "SuperAdmin",
                Description = "Tam yetkili sistem yöneticisi"
            };

            await context.Set<AppRole>().AddAsync(adminRole);
            await context.SaveChangesAsync();
        }

        // 2. SuperAdmin İçin Tüm İzinleri Ekle
        var existingClaims = await context.Set<AppRoleClaim>()
            .Where(c => c.RoleId == adminRole.Id && !c.IsDeleted)
            .Select(c => c.ClaimValue)
            .ToListAsync();

        var allPermissions = Permissions.GetAllPermissions();

        foreach (var permission in allPermissions)
        {
            if (!existingClaims.Contains(permission))
            {
                await context.Set<AppRoleClaim>().AddAsync(new AppRoleClaim
                {
                    RoleId = adminRole.Id,
                    ClaimType = "Permission",
                    ClaimValue = permission
                });
            }
        }

        await context.SaveChangesAsync();
    }
}