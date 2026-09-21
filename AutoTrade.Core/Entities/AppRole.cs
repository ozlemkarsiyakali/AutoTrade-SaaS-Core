namespace AutoTrade.Core.Entities;

public class AppRole : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation Properties
    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
    public ICollection<AppRoleClaim> RoleClaims { get; set; } = new List<AppRoleClaim>();
}