namespace AutoTrade.Core.Entities;

public class AppRoleClaim : BaseEntity
{
    public Guid RoleId { get; set; }
    public string ClaimType { get; set; } = "Permission";
    public string ClaimValue { get; set; } = string.Empty; // Örn: "VehicleBrands.Create"

    // Navigation Property
    public AppRole Role { get; set; } = null!;
}